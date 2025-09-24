using System.IO;
using VfxEditor.Formats.ShpkFormat.Utils;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.ShpkFormat.Nodes {
    public class ShpkPass : IUiItem {
        public readonly ParsedCrc Id = new( "Id " );
        public readonly ParsedUInt VertexShaderIdx = new( "Vertex Shader Index" );
        public readonly ParsedUInt PixelShaderIdx = new( "Pixel Shader Index" );
        public readonly ParsedInt Unknown1 = new( "Unknown1" );
        public readonly ParsedInt Unknown2 = new( "Unknown2" );
        public readonly ParsedInt Unknown3 = new( "Unknown3" );
        
        public ShpkPass() { }

        public ShpkPass( BinaryReader reader ) {
            Id.Read( reader );
            VertexShaderIdx.Read( reader );
            PixelShaderIdx.Read( reader );
            Unknown1.Read( reader );
            Unknown2.Read( reader );
            Unknown3.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Id.Write( writer );
            VertexShaderIdx.Write( writer );
            PixelShaderIdx.Write( writer );
            Unknown1.Write( writer );
            Unknown2.Write( writer );
            Unknown3.Write( writer );
        }

        public void Draw() {
            Id.Draw( CrcMaps.Passes );
            VertexShaderIdx.Draw();
            PixelShaderIdx.Draw();
            Unknown1.Draw();
            Unknown2.Draw();
            Unknown3.Draw();
        }
    }
}
