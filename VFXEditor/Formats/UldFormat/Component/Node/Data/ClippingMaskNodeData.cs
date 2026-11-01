using System.IO;
using VfxEditor.Formats.UldFormat.PartList;

namespace VfxEditor.Formats.UldFormat.Component.Node.Data {
    public class ClippingMaskNodeData : UldGenericData {
        private readonly PartListSelect PartListId;
        private readonly PartItemSelect PartId;

        public ClippingMaskNodeData( UldFile file ) {
            PartListId = new( file );
            PartId = new( PartListId );
        }

        public override void Read( BinaryReader reader ) {
            PartListId.Read( reader );
            PartId.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            PartListId.Write( writer );
            PartId.Write( writer );
        }

        public override void Draw() {
            PartListId.Draw();
            PartId.Draw();
            PartId.Selected?.DrawImage( false );
        }
    }
}
