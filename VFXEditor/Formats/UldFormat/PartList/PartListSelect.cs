using VfxEditor.Parsing;

namespace VfxEditor.Formats.UldFormat.PartList {
    public class PartListSelect : ParsedIntSelect<UldPartList> {
        public PartListSelect( UldFile file ) : base( "Part List", 0,
                () => file.PartsSplitView,
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
