using System.IO;
using System.Text;

namespace VFXEditor.AVFXLib.Timeline {
    public class AVFXTimelineClip : AVFXBase {
        public string UniqueId = "LLIK";
        public readonly int[] UnknownInts = new int[] { 0, 0, 0, 0 };
        public readonly float[] UnknownFloats = new float[] { -1, 0, 0, 0 };

        public AVFXTimelineClip() : base( "Clip" ) {
        }

        /*
         * unique id : string [4] (reverse)
         * 4 int[4]
         * 4 float[4]
         * 4 string[32]
         * 
         * total = 4 + 4*4 + 4*4 + 4*32 = 164
         */

        public override void ReadContents( BinaryReader reader, int size ) {
            UniqueId = Encoding.ASCII.GetString( reader.ReadBytes( 4 ) );

            for( var i = 0; i < 4; i++ ) {
                UnknownInts[i] = reader.ReadInt32();
            }

            for( var i = 0; i < 4; i++ ) {
                UnknownFloats[i] = reader.ReadSingle();
            }

            reader.ReadBytes( 4 * 32 );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            writer.Write( Encoding.ASCII.GetBytes( UniqueId ) );

            for( var i = 0; i < 4; i++ ) {
                writer.Write( UnknownInts[i] );
            }

            for( var i = 0; i < 4; i++ ) {
                writer.Write( UnknownFloats[i] );
            }

            WritePad( writer, 4 * 32 );
        }
    }
}
