using System.IO;
using VfxEditor.Formats.ShpkFormat.Utils;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.ShpkFormat.Nodes {
    public class ShpkPass : IUiItem {
        public readonly ParsedCrc Id = new( "Id " );
        public readonly ParsedUInt VertexShaderIdx = new( "Vertex Shader Index" );
        public readonly ParsedUInt PixelShaderIdx = new( "Pixel SHader Index" );

        public ShpkPass() { }

        public ShpkPass( BinaryReader reader ) {
            Id.Read( reader );
            VertexShaderIdx.Read( reader );
            PixelShaderIdx.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Id.Write( writer );
            VertexShaderIdx.Write( writer );
            PixelShaderIdx.Write( writer );
        }

        public void Draw() {
            Id.Draw( CommandManager.Shpk, CrcMaps.Passes );
            VertexShaderIdx.Draw( CommandManager.Shpk );
            PixelShaderIdx.Draw( CommandManager.Shpk );
        }
    }
}
