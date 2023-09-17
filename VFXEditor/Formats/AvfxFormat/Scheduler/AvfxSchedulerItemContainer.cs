using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AvfxFormat {
    public class AvfxSchedulerItemContainer : AvfxBase {
        public readonly AvfxScheduler Scheduler;
        public readonly List<AvfxSchedulerItem> Items = new();

        public AvfxSchedulerItemContainer( string name, AvfxScheduler scheduler ) : base( name ) {
            Scheduler = scheduler;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            for( var i = 0; i < size / 36; i++ ) {
                Items.Add( new AvfxSchedulerItem( Scheduler, false, reader, AvfxName ) );
            }
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        public override void WriteContents( BinaryWriter writer ) {
            foreach( var item in Items ) item.Write( writer );
        }
    }
}
