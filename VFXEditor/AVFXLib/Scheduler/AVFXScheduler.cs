using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Scheduler {
    public class AVFXScheduler : AVFXBase {
        public const string NAME = "Schd";

        public readonly AVFXInt ItemCount = new( "ItCn" );
        public readonly AVFXInt TriggerCount = new( "TrCn" );

        public readonly List<AVFXSchedulerSubItem> Items = new();
        public readonly List<AVFXSchedulerSubItem> Triggers = new();

        private readonly List<AVFXBase> Children;

        public AVFXScheduler() : base( NAME ) {
            Children = new List<AVFXBase> {
                ItemCount,
                TriggerCount
            };
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Peek( reader, Children, size );

            AVFXSchedulerItem lastItem = null;
            AVFXSchedulerItem lastTrigger = null;

            ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
                if( _name == "Item" ) {
                    lastItem = new AVFXSchedulerItem( "Item" );
                    lastItem.Read( _reader, _size );

                }
                else if( _name == "Trgr" ) {
                    lastTrigger = new AVFXSchedulerItem( "Trgr" );
                    lastTrigger.Read( _reader, _size );

                }
            }, size );

            if( lastItem != null ) {
                Items.AddRange( lastItem.Items );
            }
            if( lastTrigger != null ) {
                Triggers.AddRange( lastTrigger.Items.GetRange( lastTrigger.Items.Count - 12, 12 ) );
            }
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) {
            WriteNested( writer, Children );

            // Item
            for( var i = 0; i < Items.Count; i++ ) {
                var item = new AVFXSchedulerItem( "Item" );
                item.Items.AddRange( Items.GetRange( 0, i + 1 ) );
                item.Write( writer );
            }

            // Trgr
            for( var i = 0; i < Triggers.Count; i++ ) {
                var trigger = new AVFXSchedulerItem( "Trgr" );
                trigger.Items.AddRange( Items );
                trigger.Items.AddRange( Triggers.GetRange( 0, i + 1 ) ); // get 1, then 2, etc.
                trigger.Write( writer );
            }
        }

        public AVFXSchedulerSubItem Add() {
            var ItPr = new AVFXSchedulerSubItem();
            Items.Add( ItPr );
            ItemCount.SetValue( Items.Count );
            return ItPr;
        }

        public void Add( AVFXSchedulerSubItem item ) {
            Items.Add( item );
            ItemCount.SetValue( Items.Count );
        }

        public void Remove( int idx ) {
            Items.RemoveAt( idx );
            ItemCount.SetValue( Items.Count );
        }

        public void Remove( AVFXSchedulerSubItem item ) {
            Items.Remove( item );
            ItemCount.SetValue( Items.Count );
        }
    }
}
