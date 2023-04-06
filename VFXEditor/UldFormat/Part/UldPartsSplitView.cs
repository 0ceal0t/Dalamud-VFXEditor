using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;

namespace VfxEditor.UldFormat.Part {
    public class UldPartsSplitView : SimpleSplitView<UldParts> {
        public UldPartsSplitView( List<UldParts> items ) : base( "Parts", items, true ) { }

        protected override void OnNew() {
            CommandManager.Uld.Add( new GenericAddCommand<UldParts>( Items, new UldParts() ) );
        }

        protected override void OnDelete( UldParts item ) {
            CommandManager.Uld.Add( new GenericRemoveCommand<UldParts>( Items, item ) );
        }
    }
}
