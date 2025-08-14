using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.IO;
using System.Numerics;
using VfxEditor.Formats.ShpkFormat.Shaders;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MtrlFormat.Shader {
    public enum TextureAddressMode : int {
        Wrap = 0,
        Mirror = 1,
        Clamp = 2,
        Border = 3,
    }

    public class MtrlSampler : IUiItem {
        private readonly MtrlFile File;

        public readonly ParsedUIntPicker<ShpkParameterInfo> Id;
        public readonly ParsedEnum<TextureAddressMode> AddressModeU = new( "U Address Mode" );
        public readonly ParsedEnum<TextureAddressMode> AddressModeV = new( "V Address Mode" );
        public readonly ParsedFloat LoDBias = new( "LoD Bias" );
        public readonly ParsedUInt MinLoD = new( "Minimum LoD" );
        public readonly ParsedUIntHex Flags = new( "Flags" );
        public readonly ParsedByte TextureIndex = new( "Texture Index" );
        private readonly ParsedReserve Padding = new( 3 );

        public MtrlSampler( MtrlFile file ) {
            File = file;
            Id = new( "Sampler",
                () => File.ShaderFile?.Samplers,
                ( ShpkParameterInfo item, int _ ) => item.GetText(),
                ( ShpkParameterInfo item ) => item.Id
            );
        }

        public MtrlSampler( MtrlFile file, BinaryReader reader ) : this( file ) {
            Id.Read( reader );
            var flags = reader.ReadUInt32();
            TextureIndex.Read( reader );
            Padding.Read( reader );

            // Flags are 32 bits:
            //  2 U
            //  2 V
            //  6 ??????
            //  10 LoD Bias
            //  4 Min LoD
            //  8 ??????

            Flags.Value = Masked( flags );
            AddressModeU.Value = ( TextureAddressMode )( flags & 0x3u );
            AddressModeV.Value = ( TextureAddressMode )( ( flags >> 2 ) & 0x3u );
            LoDBias.Value = ( ( flags >> 10 ) & 0x3FF ) / 64f;
            MinLoD.Value = ( ( flags >> 20 ) & 0xF );
        }

        public void Write( BinaryWriter writer ) {
            Id.Write( writer );

            var flags = Masked( Flags.Value );
            flags |= ( uint )AddressModeU.Value;
            flags |= ( uint )AddressModeV.Value << 2;
            flags |= ( uint )( ( int )( LoDBias.Value * 64.0f ) & 0x3FF ) << 10;
            flags |= MinLoD.Value << 20;
            writer.Write( flags );

            TextureIndex.Write( writer );
            Padding.Write( writer );
        }

        public void Draw() {
            Id.Draw();
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 4, 2 ) ) ) {
                ImGui.SameLine();
            }
            ImGui.TextDisabled( $"[{File.ShaderFilePath.Split( '/' )[^1]}]" );

            AddressModeU.Draw();
            AddressModeV.Draw();
            LoDBias.Draw();
            MinLoD.Draw();
            Flags.Draw();
            TextureIndex.Draw();
        }

        public string GetText( int idx ) {
            var selected = Id.Selected;
            if( selected != null ) return selected.GetText();
            return $"Sampler {idx}";
        }

        private static uint Masked( uint flags ) => flags & ( ~( 0x3u | ( 0x3u << 2 ) | ( 0x3FF << 10 ) | ( 0xF << 20 ) ) );
    }
}
