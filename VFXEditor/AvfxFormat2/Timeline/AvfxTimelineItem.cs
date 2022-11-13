using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxTimelineItem : AvfxBase {
        public readonly AvfxTimeline Timeline;
        public readonly List<AvfxTimelineSubItem> Items = new();

        public AvfxTimelineItem( AvfxTimeline timeline ) : base( "Item" ) {
            Timeline = timeline;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            var split = new List<byte>();
            var numElements = size / 12; // each AVFXBool or AVFXInt is 12 bytes

            for( var idx = 0; idx < numElements; idx++ ) {
                split.AddRange( reader.ReadBytes( 12 ) );

                var BENA_ahead = false;
                if( idx != numElements - 1 ) {
                    var resetPos = reader.BaseStream.Position;
                    var nextName = ReadAvfxName( reader );
                    reader.BaseStream.Seek( resetPos, SeekOrigin.Begin );

                    BENA_ahead = nextName == "bEna";
                }

                if( ( idx == numElements - 1 ) || BENA_ahead ) { // split before bEna or when about to end
                    var item = new AvfxTimelineSubItem( Timeline, split.ToArray() );
                    Items.Add( item );
                    split = new(); // reset split
                }
            }
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            foreach( var item in Items ) item.Write( writer );
        }
    }
}
