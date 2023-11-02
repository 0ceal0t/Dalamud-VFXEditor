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
using VfxEditor.Formats.ShpkFormat.Keys;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MtrlFormat {
    // https://github.com/Ottermandias/Penumbra.GameData/blob/04ddadb44600a382e26661e1db08fd16c3b671d8/Files/MtrlFile.cs#L7

    [Flags]
    public enum TableFlags {
        HasTable = 0x4,
        HasDyeTable = 0x8
    }

    public class MtrlFile : FileManagerFile {
        private readonly byte[] Version;
        private readonly byte[] ExtraData;

        private readonly List<MtrlTexture> Textures = new();
        private readonly List<MtrlAttributeSet> UvSets = new();
        private readonly List<MtrlAttributeSet> ColorSets = new();

        private readonly ParsedString Shader = new( "Shader" );
        private readonly ParsedFlag<TableFlags> Flags = new( "Flags", 1 );

        private readonly MtrlColorTable ColorTable;
        private readonly MtrlDyeTable DyeTable;

        private readonly ParsedUInt ShaderFlags = new( "Shader Flags" );

        private readonly List<ShpkKey> Keys = new();
        private readonly List<MtrlConstant> Constants = new();
        private readonly List<MtrlSampler> Samplers = new();

        private readonly CommandSplitView<MtrlTexture> TextureView;
        private readonly CommandSplitView<MtrlAttributeSet> UvSetView;
        private readonly CommandSplitView<MtrlAttributeSet> ColorSetView;
        private readonly CommandSplitView<ShpkKey> KeyView;
        private readonly CommandSplitView<MtrlConstant> ConstantView;
        private readonly CommandSplitView<MtrlSampler> SamplerView;

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
            Shader.Value = FileUtils.ReadString( reader );

            reader.BaseStream.Seek( stringsStart + stringSize, SeekOrigin.Begin );

            // ===============

            // Otherwise, extra data is null
            if( extraDataSize > 0 ) {
                Flags.Read( reader );
                ExtraData = reader.ReadBytes( extraDataSize - 1 );
            }

            var dataEnd = reader.BaseStream.Position + dataSize;
            ColorTable = ( Flags.HasFlag( TableFlags.HasTable ) && ( dataEnd - reader.BaseStream.Position ) >= MtrlColorTable.Size ) ? new( reader ) : new();
            DyeTable = ( Flags.HasFlag( TableFlags.HasDyeTable ) && ( dataEnd - reader.BaseStream.Position ) >= MtrlDyeTable.Size ) ? new( reader ) : new();
            reader.BaseStream.Seek( dataEnd, SeekOrigin.Begin );

            var shaderValueSize = reader.ReadUInt16();
            var shaderKeyCount = reader.ReadUInt16();
            var constantCount = reader.ReadUInt16();
            var samplerCount = reader.ReadUInt16();

            ShaderFlags.Read( reader ); // TODO: look into these flags more

            for( var i = 0; i < shaderKeyCount; i++ ) Keys.Add( new( reader ) );
            for( var i = 0; i < constantCount; i++ ) Constants.Add( new( reader ) );
            for( var i = 0; i < samplerCount; i++ ) Samplers.Add( new( reader ) );

            var shaderValues = new List<float>();
            for( var i = 0; i < shaderValueSize / 4; i++ ) shaderValues.Add( reader.ReadSingle() );
            Constants.ForEach( x => x.PickValues( shaderValues ) );

            // ======== VIEWS =========

            TextureView = new( "Texture", Textures, false, ( MtrlTexture item, int idx ) => item.Text, () => new() );
            UvSetView = new( "UV Set", UvSets, false, ( MtrlAttributeSet item, int idx ) => item.Name.Value, () => new() );
            ColorSetView = new( "Color Set", ColorSets, false, ( MtrlAttributeSet item, int idx ) => item.Name.Value, () => new() );
            KeyView = new( "Key", Keys, false, ( ShpkKey item, int idx ) => item.GetText( idx ), () => new() );
            ConstantView = new( "Constant", Constants, false, null, () => new() );
            SamplerView = new( "Sampler", Samplers, false, null, () => new() );

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
            if( Flags.HasFlag( TableFlags.HasTable ) ) ColorTable.Write( writer );
            if( Flags.HasFlag( TableFlags.HasDyeTable ) ) DyeTable.Write( writer );
            var dataEnd = writer.BaseStream.Position;

            writer.Write( ( ushort )Constants.Select( x => x.Values.Count * 4 ).Sum() );
            writer.Write( ( ushort )Keys.Count );
            writer.Write( ( ushort )Constants.Count );
            writer.Write( ( ushort )Samplers.Count );

            ShaderFlags.Write( writer );

            Keys.ForEach( x => x.Write( writer ) );
            var constantPositions = new List<long>();
            Constants.ForEach( x => x.Write( writer, constantPositions ) );
            Samplers.ForEach( x => x.Write( writer ) );

            var shaderValueStart = writer.BaseStream.Position;
            foreach( var (constant, idx) in Constants.WithIndex() ) {
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

            if( Flags.HasFlag( TableFlags.HasTable ) ) {
                using var tab = ImRaii.TabItem( "Color Table" );
                if( tab ) ColorTable.Draw();
            }

            if( Flags.HasFlag( TableFlags.HasDyeTable ) ) {
                using var tab = ImRaii.TabItem( "Dye Table" );
                if( tab ) DyeTable.Draw();
            }
        }

        private void DrawParameters() {
            using var _ = ImRaii.PushId( "Shader" );

            Shader.Draw();
            ShaderFlags.Draw();
            Flags.Draw();

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Keys" ) ) {
                if( tab ) KeyView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Constants" ) ) {
                if( tab ) ConstantView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Samplers" ) ) {
                if( tab ) SamplerView.Draw();
            }
        }
    }
}