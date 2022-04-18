using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Timeline {
    public class AVFXTimelineItem : AVFXBase {
        public readonly List<AVFXTimelineSubItem> Items = new();

        public AVFXTimelineItem() : base( "Item" ) {
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            var split = new List<byte>();
            var numElements = size / 12; // each AVFXBool or AVFXInt is 12 bytes

            for(var idx = 0; idx < numElements; idx++) {
                split.AddRange( reader.ReadBytes( 12 ) );

                var BENA_ahead = false;
                if (idx != numElements - 1) {
                    var resetPos = reader.BaseStream.Position;
                    var nextName = ReadName( reader );
                    reader.BaseStream.Seek( resetPos, SeekOrigin.Begin );

                    BENA_ahead = nextName == "bEna";
                }

                if ( (idx == numElements - 1) || BENA_ahead ) { // split before bEna or when about to end
                    var item = new AVFXTimelineSubItem( split.ToArray() );
                    Items.Add( item );
                    split = new(); // reset split
                }
            }
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            foreach( var item in Items ) {
                item.Write( writer );
            }
        }
    }

    public class AVFXTimelineSubItem {
        public readonly AVFXBool Enabled = new( "bEna" );
        public readonly AVFXInt StartTime = new( "StTm" );
        public readonly AVFXInt EndTime = new( "EdTm" );
        public readonly AVFXInt BinderIdx = new( "BdNo" );
        public readonly AVFXInt EffectorIdx = new( "EfNo" );
        public readonly AVFXInt EmitterIdx = new( "EmNo" );
        public readonly AVFXInt Platform = new( "Plfm" );
        public readonly AVFXInt ClipNumber = new( "ClNo" );

        private readonly List<AVFXBase> Children;

        public AVFXTimelineSubItem() {
            Children = new List<AVFXBase> {
                Enabled,
                StartTime,
                EndTime,
                BinderIdx,
                EffectorIdx,
                EmitterIdx,
                Platform,
                ClipNumber
            };
            AVFXBase.RecurseAssigned( Children, false );
        }

        public AVFXTimelineSubItem( byte[] data ) : this() {
            using var buffer = new MemoryStream( data );
            using var reader = new BinaryReader( buffer );
            AVFXBase.ReadNested( reader, Children, data.Length );
        }

        public void Write( BinaryWriter writer ) {
            AVFXBase.WriteNested( writer, Children );
        }
    }
}
