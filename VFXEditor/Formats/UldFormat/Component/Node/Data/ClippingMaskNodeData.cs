using System.IO;
using VfxEditor.Parsing.Int;
using VfxEditor.UldFormat.PartList;

namespace VfxEditor.UldFormat.Component.Node.Data {
    public class ClippingMaskNodeData : UldGenericData {
        private readonly ParsedIntSelect<UldPartList> PartListId;
        private readonly ParsedUIntPicker<UldPartItem> PartId;

        public ClippingMaskNodeData() {
            PartListId = new( "Part List", 0,
                () => Plugin.UldManager.File.PartsSplitView,
                ( UldPartList item ) => ( int )item.Id.Value,
                ( UldPartList item, int _ ) => item.GetText()
            );
            PartId = new( "Part",
                () => PartListId.Selected?.Parts,
                ( UldPartItem item, int idx ) => item.GetText( idx ),
                null
            );
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
