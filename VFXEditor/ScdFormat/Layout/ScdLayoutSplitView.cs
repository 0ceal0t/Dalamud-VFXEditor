using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.FileManager;

namespace VfxEditor.ScdFormat.Layout {
    public class ScdLayoutSplitView : ScdSimpleSplitView<ScdLayoutEntry> {
        public ScdLayoutSplitView( List<ScdLayoutEntry> items ) : base( "Layout", items, true ) {
        }

        protected override void OnNew() {
            CommandManager.Scd.Add( new GenericAddCommand<ScdLayoutEntry>( Items, new ScdLayoutEntry() ) );
        }

        protected override void OnDelete( ScdLayoutEntry item ) {
            CommandManager.Scd.Add( new GenericRemoveCommand<ScdLayoutEntry>( Items, item ) );
        }
    }
}
