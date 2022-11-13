using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxSchedulerItem : AvfxBase {
        public readonly AvfxScheduler Scheduler;
        public readonly List<AvfxSchedulerSubItem> Items = new();

        public AvfxSchedulerItem( string name, AvfxScheduler scheduler ) : base( name ) {
            Scheduler = scheduler;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            for( var i = 0; i < size / 36; i++ ) {
                Items.Add( new AvfxSchedulerSubItem( Scheduler, reader, AvfxName ) );
            }
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            foreach( var item in Items ) item.Write( writer );
        }
    }
}
