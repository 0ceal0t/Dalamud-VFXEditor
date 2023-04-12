using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;

namespace VfxEditor.UldFormat.PartList {
    public class UldPartsSplitView : SimpleSplitView<UldPartList> {
        public UldPartsSplitView( List<UldPartList> items ) : base( "Part Lists", items, true ) { }

        protected override void OnNew() {
            CommandManager.Uld.Add( new GenericAddCommand<UldPartList>( Items, new UldPartList() ) );
        }

        protected override void OnDelete( UldPartList item ) {
            CommandManager.Uld.Add( new GenericRemoveCommand<UldPartList>( Items, item ) );
        }

        protected override string GetText( UldPartList item, int idx ) => item.GetText();
    }
}
