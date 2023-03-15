using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.FileManager;

namespace VfxEditor.ScdFormat.Track {
    public class ScdTrackSplitView : ScdSimpleSplitView<ScdTrackEntry> {
        public ScdTrackSplitView( List<ScdTrackEntry> items ) : base( "Track", items, true ) {
        }

        protected override void OnNew() {
            CommandManager.Scd.Add( new GenericAddCommand<ScdTrackEntry>( Items, new ScdTrackEntry() ) );
        }

        protected override void OnDelete( ScdTrackEntry item ) {
            CommandManager.Scd.Add( new GenericRemoveCommand<ScdTrackEntry>( Items, item ) );
        }
    }
}
