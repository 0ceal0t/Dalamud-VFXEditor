using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Formats.MtrlFormat.AttributeSet;
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

        private readonly CommandSplitView<MtrlTexture> TextureView;
        private readonly CommandSplitView<MtrlAttributeSet> UvSetView;
        private readonly CommandSplitView<MtrlAttributeSet> ColorSetView;
        private readonly CommandSplitView<ShpkKey> KeyView;

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

            /*
             * public struct Constant
    {
        public uint ConstantId;
        public ushort ValueOffset;
        public ushort ValueSize;
    }

    public unsafe struct Sampler
    {
        public uint SamplerId;
        public uint Flags; // Bitfield; values unknown
        public byte TextureIndex;
        private fixed byte Padding[3];
    }
             */

            for( var i = 0; i < shaderKeyCount; i++ ) Keys.Add( new( reader ) );

            //ShaderPackage.Constants = r.Read<Constant>( constantCount ).ToArray();
            //ShaderPackage.Samplers = r.Read<Sampler>( samplerCount ).ToArray();
            //ShaderPackage.ShaderValues = r.Read<float>( shaderValueListSize / 4 ).ToArray();

            // ======== VIEWS =========

            TextureView = new( "Texture", Textures, false, ( MtrlTexture item, int idx ) => item.Text, () => new() );
            UvSetView = new( "UV Set", UvSets, false, ( MtrlAttributeSet item, int idx ) => item.Name.Value, () => new() );
            ColorSetView = new( "Color Set", ColorSets, false, ( MtrlAttributeSet item, int idx ) => item.Name.Value, () => new() );
            KeyView = new( "Key", Keys, false, ( ShpkKey item, int idx ) => item.GetText( idx ), () => new() );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Version );

            // TODO
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
        }
    }
}
