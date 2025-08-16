using VfxEditor.Parsing.Int;
using VfxEditor.UldFormat.PartList;

namespace VfxEditor.Formats.UldFormat.PartList {
    public class PartListSelect : ParsedIntSelect<UldPartList> {
        public PartListSelect() : base( "Part List", 0,
                () => Plugin.UldManager.File.PartsSplitView,
                item => ( int )item.Id.Value,
                ( item, _ ) => item.GetText() ) { }
    }

    public class PartItemSelect : ParsedUIntPicker<UldPartItem> {
        public PartItemSelect( PartListSelect partListSelect ) : base( "Part",
                () => partListSelect.Selected?.Parts,
                ( item, idx ) => item.GetText( idx ),
                null ) { }
    }
}
