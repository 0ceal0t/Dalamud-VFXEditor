using System;
using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AvfxFormat {
    public class AvfxSchedulerItem : AvfxBase {
        public readonly AvfxScheduler Scheduler;
        public readonly List<AvfxSchedulerSubItem> Items = new();

        public AvfxSchedulerItem( string name, AvfxScheduler scheduler ) : base( name ) {
            Scheduler = scheduler;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            for( var i = 0; i < size / 36; i++ ) {
                Items.Add( new AvfxSchedulerSubItem( Scheduler, false, reader, AvfxName ) );
            }
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            foreach( var item in Items ) item.Write( writer );
        }
    }
}
