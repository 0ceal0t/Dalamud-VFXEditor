using System.IO;
using VfxEditor.Formats.UldFormat.PartList;
using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Node.Data {
    public class ImageNodeData : UldGenericData {
        private readonly PartListSelect PartListId;
        private readonly PartItemSelect PartId;

        private readonly ParsedUInt Unknown1 = new( "Unknown 1", size: 2 );
        private readonly ParsedByteBool FlipH = new( "Flip H" );
        private readonly ParsedByteBool FlipV = new( "Flip V" );
        private readonly ParsedInt Wrap = new( "Wrap", size: 1 );
        private readonly ParsedInt DrawMode = new( "Draw Mode", size: 1 );

        public ImageNodeData() {
            PartListId = new();
            PartId = new( PartListId );
        }

        public override void Read( BinaryReader reader ) {
            PartListId.Read( reader );
            Unknown1.Read( reader );
            PartId.Read( reader );
            FlipH.Read( reader );
            FlipV.Read( reader );
            Wrap.Read( reader );
            DrawMode.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            PartListId.Write( writer );
            Unknown1.Write( writer );
            PartId.Write( writer );
            FlipH.Write( writer );
            FlipV.Write( writer );
            Wrap.Write( writer );
            DrawMode.Write( writer );
        }

        public override void Draw() {
            PartListId.Draw();
            PartId.Draw();
            PartId.Selected?.DrawImage( false );

            FlipH.Draw();
            FlipV.Draw();
            Wrap.Draw();
            Unknown1.Draw();
            DrawMode.Draw();
        }
    }
}
