using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;

namespace VfxEditor.UldFormat.Timeline.Frames {
    public class UldKeyGroupSplitView : SimpleSplitView<UldKeyGroup> {
        public UldKeyGroupSplitView( List<UldKeyGroup> items ) : base( "Key Group", items, true, true ) { }

        protected override void OnNew() {
            CommandManager.Uld.Add( new GenericAddCommand<UldKeyGroup>( Items, new UldKeyGroup() ) );
        }

        protected override void OnDelete( UldKeyGroup item ) {
            CommandManager.Uld.Add( new GenericRemoveCommand<UldKeyGroup>( Items, item ) );
        }
    }
}
