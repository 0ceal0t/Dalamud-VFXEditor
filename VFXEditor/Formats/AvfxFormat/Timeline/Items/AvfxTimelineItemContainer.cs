using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AvfxFormat {
    public class AvfxTimelineItemContainer : AvfxBase {
        public readonly AvfxTimeline Timeline;
        public readonly List<AvfxTimelineItem> Items = new();

        public AvfxTimelineItemContainer( AvfxTimeline timeline ) : base( "Item" ) {
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
                    var item = new AvfxTimelineItem( Timeline, false, split.ToArray() );
                    Items.Add( item );
                    split = new(); // reset split
                }
            }
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        public override void WriteContents( BinaryWriter writer ) {
            foreach( var item in Items ) item.Write( writer );
        }
    }
}
