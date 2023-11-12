using Dalamud.Interface;
using ImGuiNET;
using OtterGui;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.FileManager;
using VfxEditor.Formats.MtrlFormat.AttributeSet;
using VfxEditor.Formats.MtrlFormat.Shader;
using VfxEditor.Formats.MtrlFormat.Table;
using VfxEditor.Formats.MtrlFormat.Texture;
using VfxEditor.Formats.ShpkFormat;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MtrlFormat {
    // https://github.com/Ottermandias/Penumbra.GameData/blob/04ddadb44600a382e26661e1db08fd16c3b671d8/Files/MtrlFile.cs#L7

    [Flags]
    public enum TableFlags {
        Has_Color_Table = 0x4,
        Has_Dye_Table = 0x8
    }

    [Flags]
    public enum ShaderFlagOptions {
        Hide_Backfaces = 0x01,
        Enable_Transparency = 0x10
    }

    public enum ShpkFileState {
        Unloaded,
        None,
        Penumbra,
        Replaced,
        Missing
    }

    public class MtrlFile : FileManagerFile {
        private readonly byte[] Version;
        private readonly byte[] ExtraData;

        private readonly List<MtrlTexture> Textures = new();
        private readonly List<MtrlAttributeSet> UvSets = new();
        private readonly List<MtrlAttributeSet> ColorSets = new();

        public readonly ParsedString Shader;
        private readonly ParsedFlag<TableFlags> Flags = new( "Flags", 1 );

        public readonly MtrlColorTable ColorTable;
        public bool DyeTableEnabled => Flags.HasFlag( TableFlags.Has_Dye_Table );
        public readonly MtrlDyeTable DyeTable;

        private readonly ParsedFlag<ShaderFlagOptions> ShaderOptions = new( "Shader Options" );
        private readonly ParsedUIntHex ShaderFlags = new( "Shader Flags" );

        private readonly List<MtrlKey> Keys = new();
        private readonly List<MtrlMaterialParameter> MaterialParameters = new();
        private readonly List<MtrlSampler> Samplers = new();

        private readonly CommandSplitView<MtrlTexture> TextureView;
        private readonly CommandSplitView<MtrlAttributeSet> UvSetView;
        private readonly CommandSplitView<MtrlAttributeSet> ColorSetView;
        private readonly CommandSplitView<MtrlKey> KeyView;
        private readonly CommandSplitView<MtrlMaterialParameter> MaterialParameterView;
        private readonly CommandSplitView<MtrlSampler> SamplerView;

        public ShpkFile ShaderFile { get; private set; }
        public ShpkFileState ShaderFileState { get; private set; } = ShpkFileState.Unloaded;
        public string ShaderFilePath { get; private set; } = "";

        public MtrlFile( BinaryReader reader, bool verify ) : base() {
            Version = reader.ReadBytes( 4 );
            reader.ReadUInt16(); // file size
            var dataSize = reader.ReadUInt16();
            var stringSize = reader.ReadUInt16();
            var shaderOffset = reader.ReadUInt16(); // from start of strings
            var textureCount = reader.ReadByte();
            var uvSetCount = reader.ReadByte();
            var colorSetCount = reader.ReadByte();
            var extraDataSize = reader.ReadByte();

            for( var i = 0; i < textureCount; i++ ) Textures.Add( new( reader ) );
            for( var i = 0; i < uvSetCount; i++ ) UvSets.Add( new( reader ) );
            for( var i = 0; i < colorSetCount; i++ ) ColorSets.Add( new( reader ) );

            // ===== STRINGS =======

            var stringsStart = reader.BaseStream.Position;

            Textures.ForEach( x => x.ReadString( reader, stringsStart ) );
            UvSets.ForEach( x => x.ReadString( reader, stringsStart ) );
            ColorSets.ForEach( x => x.ReadString( reader, stringsStart ) );

            reader.BaseStream.Seek( stringsStart + shaderOffset, SeekOrigin.Begin );
            Shader = new( "Shader", new List<ParsedStringIcon>() {
                new() {
                    Icon = () => FontAwesomeIcon.Sync,
                    Remove = false,
                    Action = ( string _ ) => UpdateShaderFile()
                }
            } ) {
                Value = FileUtils.ReadString( reader )
            };

            reader.BaseStream.Seek( stringsStart + stringSize, SeekOrigin.Begin );

            // ===============

            // Otherwise, extra data is null
            if( extraDataSize > 0 ) {
                Flags.Read( reader );
                ExtraData = reader.ReadBytes( extraDataSize - 1 );
            }

            var dataEnd = reader.BaseStream.Position + dataSize;
            ColorTable = ( Flags.HasFlag( TableFlags.Has_Color_Table ) && ( dataEnd - reader.BaseStream.Position ) >= MtrlColorTable.Size ) ? new( this, reader ) : new( this );
            DyeTable = ( Flags.HasFlag( TableFlags.Has_Dye_Table ) && ( dataEnd - reader.BaseStream.Position ) >= MtrlDyeTable.Size ) ? new( reader ) : new();
            reader.BaseStream.Seek( dataEnd, SeekOrigin.Begin );

            var shaderValueSize = reader.ReadUInt16();
            var shaderKeyCount = reader.ReadUInt16();
            var constantCount = reader.ReadUInt16();
            var samplerCount = reader.ReadUInt16();

            var shaderFlags = reader.ReadUInt32();
            ShaderFlags.Value = Masked( shaderFlags );
            ShaderOptions.Value = ( ShaderFlagOptions )( shaderFlags & 0x11u );

            for( var i = 0; i < shaderKeyCount; i++ ) Keys.Add( new( reader ) );
            for( var i = 0; i < constantCount; i++ ) MaterialParameters.Add( new( this, reader ) );
            for( var i = 0; i < samplerCount; i++ ) Samplers.Add( new( this, reader ) );

            var shaderValues = new List<float>();
            for( var i = 0; i < shaderValueSize / 4; i++ ) shaderValues.Add( reader.ReadSingle() );
            MaterialParameters.ForEach( x => x.PickValues( shaderValues ) );

            // ======== VIEWS =========

            TextureView = new( "Texture", Textures, false, ( MtrlTexture item, int idx ) => item.Text, () => new() );
            UvSetView = new( "UV Set", UvSets, false, ( MtrlAttributeSet item, int idx ) => item.Name.Value, () => new() );
            ColorSetView = new( "Color Set", ColorSets, false, ( MtrlAttributeSet item, int idx ) => item.Name.Value, () => new() );
            KeyView = new( "Key", Keys, false, ( MtrlKey item, int idx ) => item.GetText( idx ), () => new() );
            MaterialParameterView = new( "Constant", MaterialParameters, false, null, () => new( this ) );
            SamplerView = new( "Sampler", Samplers, false, ( MtrlSampler item, int idx ) => item.GetText( idx ), () => new( this ) );

            if( verify ) Verified = FileUtils.Verify( reader, ToBytes(), null );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Version );
            var placeholderPos = writer.BaseStream.Position;
            writer.Write( ( ushort )0 ); // file size
            writer.Write( ( ushort )0 ); // data size
            writer.Write( ( ushort )0 ); // string size
            writer.Write( ( ushort )0 ); // shader offset

            writer.Write( ( byte )Textures.Count );
            writer.Write( ( byte )UvSets.Count );
            writer.Write( ( byte )ColorSets.Count );

            writer.Write( ( byte )( ExtraData == null ? 0 : ExtraData.Length + 1 ) ); // TODO maybe this is padding?

            var stringPositions = new Dictionary<long, string>(); // Mapping of placeholder positions to their string
            Textures.ForEach( x => x.Write( writer, stringPositions ) );
            UvSets.ForEach( x => x.Write( writer, stringPositions ) );
            ColorSets.ForEach( x => x.Write( writer, stringPositions ) );

            // =======================

            var stringStart = writer.BaseStream.Position;
            var stringOffsets = new Dictionary<string, long>(); // Mapping of written strings to their offsets

            foreach( var entry in stringPositions ) {
                if( !stringOffsets.TryGetValue( entry.Value, out var offset ) ) {
                    offset = writer.BaseStream.Position - stringStart;
                    stringOffsets[entry.Value] = offset;
                    FileUtils.WriteString( writer, entry.Value, true );
                }
                var savePos = writer.BaseStream.Position;
                writer.BaseStream.Seek( entry.Key, SeekOrigin.Begin );
                writer.Write( ( ushort )offset );
                writer.BaseStream.Seek( savePos, SeekOrigin.Begin );
            }

            var shaderOffset = writer.BaseStream.Position - stringStart;
            Shader.Write( writer );

            FileUtils.PadTo( writer, 4 );
            var stringEnd = writer.BaseStream.Position;

            // =======================

            if( ExtraData != null ) {
                Flags.Write( writer );
                writer.Write( ExtraData );
            }

            var dataStart = writer.BaseStream.Position;
            if( Flags.HasFlag( TableFlags.Has_Color_Table ) ) ColorTable.Write( writer );
            if( Flags.HasFlag( TableFlags.Has_Dye_Table ) ) DyeTable.Write( writer );
            var dataEnd = writer.BaseStream.Position;

            writer.Write( ( ushort )MaterialParameters.Select( x => x.Values.Count * 4 ).Sum() );
            writer.Write( ( ushort )Keys.Count );
            writer.Write( ( ushort )MaterialParameters.Count );
            writer.Write( ( ushort )Samplers.Count );

            var shaderFlags = Masked( ShaderFlags.Value );
            shaderFlags |= ( uint )ShaderOptions.Value;
            writer.Write( shaderFlags );

            Keys.ForEach( x => x.Write( writer ) );
            var constantPositions = new List<long>();
            MaterialParameters.ForEach( x => x.Write( writer, constantPositions ) );
            Samplers.ForEach( x => x.Write( writer ) );

            var shaderValueStart = writer.BaseStream.Position;
            foreach( var (constant, idx) in MaterialParameters.WithIndex() ) {
                var offset = writer.BaseStream.Position - shaderValueStart;
                foreach( var value in constant.Values ) writer.Write( value.Value ); // write float values

                var savePos = writer.BaseStream.Position;
                writer.BaseStream.Seek( constantPositions[idx], SeekOrigin.Begin );
                writer.Write( ( ushort )offset );
                writer.BaseStream.Seek( savePos, SeekOrigin.Begin );
            }

            // ================

            writer.BaseStream.Seek( placeholderPos, SeekOrigin.Begin );
            writer.Write( ( ushort )writer.BaseStream.Length );
            writer.Write( ( ushort )( dataEnd - dataStart ) );
            writer.Write( ( ushort )( stringEnd - stringStart ) );
            writer.Write( ( ushort )shaderOffset );
        }

        public override void Draw() {
            if( ShaderFileState == ShpkFileState.Unloaded ) UpdateShaderFile();

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Parameters" ) ) {
                if( tab ) DrawParameters();
            }

            using( var tab = ImRaii.TabItem( "Textures" ) ) {
                if( tab ) TextureView.Draw();
            }

            using( var tab = ImRaii.TabItem( "UV Sets" ) ) {
                if( tab ) UvSetView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Color Sets" ) ) {
                if( tab ) ColorSetView.Draw();
            }

            if( Flags.HasFlag( TableFlags.Has_Color_Table ) ) {
                using var tab = ImRaii.TabItem( "Color Table" );
                if( tab ) ColorTable.Draw();
            }
        }

        private void DrawParameters() {
            using var _ = ImRaii.PushId( "Shader" );

            Shader.Draw();
            ImGui.TextDisabled( ShaderFilePath );
            if( ShaderFileState != ShpkFileState.None ) {
                using var color = ImRaii.PushColor( ImGuiCol.TextDisabled, ShaderFileState == ShpkFileState.Missing ? UiUtils.DALAMUD_RED : UiUtils.PARSED_GREEN );
                ImGui.SameLine();
                ImGui.Text( $"[{ShaderFileState}]" );
            }

            ShaderFlags.Draw();
            ShaderOptions.Draw();
            Flags.Draw();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Keys" ) ) {
                if( tab ) KeyView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Material Parameters" ) ) {
                if( tab ) MaterialParameterView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Samplers" ) ) {
                if( tab ) SamplerView.Draw();
            }
        }

        private void UpdateShaderFile() {
            ShaderFile?.Dispose();
            ShaderFile = null;
            ShaderFileState = ShpkFileState.Missing;

            var path = $"shader/sm5/shpk/{Shader.Value}";
            var newPath = "";
            var state = ShpkFileState.None;

            if( Plugin.ShpkManager.GetReplacePath( path, out var replacePath ) ) {
                newPath = replacePath;
                state = ShpkFileState.Replaced;
            }
            else if( Plugin.PenumbraIpc.PenumbraFileExists( path, out var penumbraPath ) ) {
                newPath = penumbraPath;
                state = ShpkFileState.Penumbra;
            }
            else if( Dalamud.DataManager.FileExists( path ) ) {
                newPath = path;
            }

            if( string.IsNullOrEmpty( newPath ) ) return;

            try {
                var data = Path.IsPathRooted( newPath ) ? File.ReadAllBytes( newPath ) : Dalamud.DataManager.GetFile( newPath ).Data;
                using var ms = new MemoryStream( data );
                using var reader = new BinaryReader( ms );

                ShaderFile = new ShpkFile( reader, false );
                ShaderFileState = state;
                ShaderFilePath = path;
            }
            catch( Exception e ) {
                Dalamud.Error( e, $"Error reading shader {newPath}" );
            }
        }

        private static uint Masked( uint flags ) => flags & ( ~0x11u );

        public override void Dispose() {
            base.Dispose();
            if( Plugin.DirectXManager.MaterialPreview.CurrentFile == this ) {
                Plugin.DirectXManager.MaterialPreview.ClearFile();
            }
        }
    }
}
