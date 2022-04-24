using System.Collections.Generic;
using System.IO;

namespace VFXEditor.AVFXLib.Scheduler {
    public class AVFXSchedulerItem : AVFXBase {
        public readonly List<AVFXSchedulerSubItem> Items = new();

        public AVFXSchedulerItem( string name ) : base( name ) {
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            for( int i = 0; i < size / 36; i++ ) {
                Items.Add( new AVFXSchedulerSubItem( reader ) );
            }
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            foreach( var item in Items ) {
                item.Write( writer );
            }
        }
    }

    public class AVFXSchedulerSubItem {
        public readonly AVFXBool Enabled = new( "bEna" );
        public readonly AVFXInt StartTime = new( "StTm" );
        public readonly AVFXInt TimelineIdx = new( "TlNo" );

        private readonly List<AVFXBase> Children;

        public AVFXSchedulerSubItem() {
            Children = new List<AVFXBase> {
                Enabled,
                StartTime,
                TimelineIdx
            };

            Enabled.SetValue( true );
            StartTime.SetValue( 0 );
            TimelineIdx.SetValue( -1 );
        }

        public AVFXSchedulerSubItem( BinaryReader reader ) : this() {
            AVFXBase.ReadNested( reader, Children, 36 );
        }

        public void Write( BinaryWriter writer ) {
            AVFXBase.WriteNested( writer, Children );
        }
    }
}
