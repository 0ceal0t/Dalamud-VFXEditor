using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;

namespace VfxEditor.UldFormat.Timeline.Frames {
    public class UldFrameSplitView : SimpleSplitView<UldFrame> {
        public UldFrameSplitView( List<UldFrame> items ) : base( "Frame", items, true, true ) { }

        protected override void OnNew() {
            CommandManager.Uld.Add( new GenericAddCommand<UldFrame>( Items, new UldFrame() ) );
        }

        protected override void OnDelete( UldFrame item ) {
            CommandManager.Uld.Add( new GenericRemoveCommand<UldFrame>( Items, item ) );
        }
    }
}
