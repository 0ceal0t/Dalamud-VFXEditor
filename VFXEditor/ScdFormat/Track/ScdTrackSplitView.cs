using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;

namespace VfxEditor.ScdFormat.Track {
    public class ScdTrackSplitView : SimpleSplitView<ScdTrackEntry> {
        public ScdTrackSplitView( List<ScdTrackEntry> items ) : base( "Track", items, true ) { }

        protected override void OnNew() {
            CommandManager.Scd.Add( new GenericAddCommand<ScdTrackEntry>( Items, new ScdTrackEntry() ) );
        }

        protected override void OnDelete( ScdTrackEntry item ) {
            CommandManager.Scd.Add( new GenericRemoveCommand<ScdTrackEntry>( Items, item ) );
        }
    }
}
