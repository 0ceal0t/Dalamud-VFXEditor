using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Data.Command;
using VfxEditor.FileManager;
using VfxEditor.Formats.MtrlFormat.AttributeSet;
using VfxEditor.Formats.MtrlFormat.Data.Table;
using VfxEditor.Formats.MtrlFormat.Shader;
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
        Dyeable = 0x8
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
        public readonly uint Version;
        private readonly byte[] ExtraData;

        private readonly List<MtrlTexture> Textures = [];
        private readonly List<MtrlAttributeSet> UvSets = [];
        private readonly List<MtrlAttributeSet> ColorSets = [];

        public readonly ParsedString Shader;
        private readonly ParsedFlag<TableFlags> Flags = new( "Flags", 1 );

        public bool ColorTableEnabled => Flags.HasFlag( TableFlags.Has_Color_Table );
        public bool DyeTableEnabled => Flags.HasFlag( TableFlags.Dyeable );

        private MtrlTableBase Table;

        private readonly ParsedFlag<ShaderFlagOptions> ShaderOptions = new( "Shader Options" );
        private readonly ParsedUIntHex ShaderFlags = new( "Shader Flags" );

        private readonly List<MtrlKey> Keys = [];
        private readonly List<MtrlMaterialParameter> MaterialParameters = [];
        private readonly List<MtrlSampler> Samplers = [];

        private readonly CommandSplitView<MtrlTexture> TextureView;
        private readonly CommandSplitView<MtrlAttributeSet> UvSetView;
        private readonly CommandSplitView<MtrlAttributeSet> ColorSetView;
        private readonly CommandSplitView<MtrlKey> KeyView;
        private readonly CommandSplitView<MtrlMaterialParameter> MaterialParameterView;
        private readonly CommandSplitView<MtrlSampler> SamplerView;

        public ShpkFile ShaderFile { get; private set; }
        public ShpkFileState ShaderFileState { get; private set; } = ShpkFileState.Unloaded;
        public string ShaderFilePath { get; private set; } = "";

        private readonly int ModdedMod4 = 0;

        public MtrlFile( BinaryReader reader, bool verify ) : base() {
            Version = reader.ReadUInt32();
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

            // TODO: https://github.com/goaaats/ffxiv-explorer-fork/blob/fcf2e0167a8b1af642b40ea67d85113234d8b05c/src/main/java/com/fragmenterworks/ffxivextract/models/Model.java#L145

            var stringsStart = reader.BaseStream.Position;

            Textures.ForEach( x => x.ReadString( reader, stringsStart ) );
            UvSets.ForEach( x => x.ReadString( reader, stringsStart ) );
            ColorSets.ForEach( x => x.ReadString( reader, stringsStart ) );

            reader.BaseStream.Position = stringsStart + shaderOffset;
            Shader = new( "Shader", new List<ParsedStringIcon>() {
                new() {
                    Icon = () => FontAwesomeIcon.Sync,
                    Remove = false,
                    Action = ( string _ ) => UpdateShaderFile()
                }
            } ) {
                Value = FileUtils.ReadString( reader )
            };

            reader.BaseStream.Position = stringsStart + stringSize;
            ModdedMod4 = ( int )( reader.BaseStream.Position % 4 );

            // ===============

            // Otherwise, extra data is null
            if( extraDataSize > 0 ) {
                Flags.Read( reader );
                ExtraData = reader.ReadBytes( extraDataSize - 1 );
            }

            var dataEnd = reader.BaseStream.Position + dataSize;
            var size = ( int )( dataEnd - reader.BaseStream.Position );
            Table = ( !ColorTableEnabled || size < ( int )ColorTableSize.Legacy ) ?
                null : // default
                ( !( size >= ( int )ColorTableSize.Extended ) ?
                    new MtrlTableLegacy( this, reader, dataEnd ) :
                    new MtrlTable( this, reader, dataEnd )
                );

            reader.BaseStream.Position = dataEnd;

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

            if( verify ) Verified = FileUtils.Verify( reader, ToBytes() );
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
                writer.BaseStream.Position = entry.Key;
                writer.Write( ( ushort )offset );
                writer.BaseStream.Position = savePos;
            }

            var shaderOffset = writer.BaseStream.Position - stringStart;
            Shader.Write( writer );

            FileUtils.PadTo( writer, writer.BaseStream.Position, 4, ModdedMod4 );

            var stringEnd = writer.BaseStream.Position;

            // =======================

            if( ExtraData != null ) {
                Flags.Write( writer );
                writer.Write( ExtraData );
            }

            var dataStart = writer.BaseStream.Position;
            Table?.Write( writer );
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
                writer.BaseStream.Position = constantPositions[idx];
                writer.Write( ( ushort )offset );
                writer.BaseStream.Position = savePos;
            }

            // ================

            writer.BaseStream.Position = placeholderPos;
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

            if( ColorTableEnabled && Table != null ) {
                using var tab = ImRaii.TabItem( "Color Table" );
                if( tab ) Table?.Draw();
            }
        }

        private void DrawParameters() {
            using var _ = ImRaii.PushId( "Shader" );
            using var child = ImRaii.Child( "Child" );

            Shader.Draw();
            ImGui.TextDisabled( ShaderFilePath );
            if( ShaderFileState != ShpkFileState.None ) {
                using var color = ImRaii.PushColor( ImGuiCol.Text, ShaderFileState == ShpkFileState.Missing ? UiUtils.DALAMUD_RED : UiUtils.PARSED_GREEN );
                ImGui.SameLine();
                ImGui.Text( $"[{ShaderFileState}]" );
            }
            ShaderFlags.Draw();
            ShaderOptions.Draw();
            using( var edited = new Edited() ) {
                Flags.Draw();
                if( edited.IsEdited && ColorTableEnabled && Table == null ) Table = new MtrlTable( this );
            }

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
    }
}
