using ImGuiNET;
using System.IO;
using VfxEditor.Formats.ShpkFormat.Utils;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MtrlFormat.Shader {
    public enum TextureAddressMode : int {
        Wrap = 0,
        Mirror = 1,
        Clamp = 2,
        Border = 3,
    }

    public class MtrlSampler : IUiItem {
        public readonly ParsedCrc Id = new( "Id" );
        public readonly ParsedEnum<TextureAddressMode> AddressModeU = new( "U Address Mode" );
        public readonly ParsedEnum<TextureAddressMode> AddressModeV = new( "V Address Mode" );
        public readonly ParsedFloat LoDBias = new( "LoD Bias" );
        public readonly ParsedUInt MinLoD = new( "Minimum LoD" );
        public readonly ParsedByte TextureIndex = new( "Texture Index" );
        private readonly ParsedReserve Padding = new( 3 );

        private readonly uint OriginalFlags = 0;
        private uint MaskedFlags => OriginalFlags & ( ~( 0x3u | ( 0x3u << 2 ) | ( 0x3FF << 10 ) | ( 0xF << 20 ) ) );

        public MtrlSampler() { }

        public MtrlSampler( BinaryReader reader ) {
            Id.Read( reader );
            OriginalFlags = reader.ReadUInt32();
            TextureIndex.Read( reader );
            Padding.Read( reader );

            // Flags are 32 bits:
            //  2 U
            //  2 V
            //  6 ??????
            //  10 LoD Bias
            //  4 Min LoD
            //  8 ??????

            AddressModeU.Value = ( TextureAddressMode )( OriginalFlags & 0x3u );
            AddressModeV.Value = ( TextureAddressMode )( ( OriginalFlags >> 2 ) & 0x3u );
            LoDBias.Value = ( ( OriginalFlags >> 10 ) & 0x3FF ) / 64f;
            MinLoD.Value = ( ( OriginalFlags >> 20 ) & 0xF );
        }

        public void Write( BinaryWriter writer ) {
            Id.Write( writer );

            var flags = MaskedFlags;
            flags |= ( uint )AddressModeU.Value;
            flags |= ( uint )AddressModeV.Value << 2;
            flags |= ( uint )( ( int )( LoDBias.Value * 64.0f ) & 0x3FF ) << 10;
            flags |= MinLoD.Value << 20;
            writer.Write( flags );

            TextureIndex.Write( writer );
            Padding.Write( writer );
        }

        public void Draw() {
            Id.Draw( CrcMaps.MaterialParams ); // TODO
            AddressModeU.Draw();
            AddressModeV.Draw();
            LoDBias.Draw();
            MinLoD.Draw();
            ImGui.TextDisabled( $"Extra Flags: 0x{MaskedFlags:X8}" );
            TextureIndex.Draw();
        }
    }
}
