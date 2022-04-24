using System.IO;

namespace VFXEditor.AVFXLib {
    public class AVFXInt : AVFXBase {
        private int Size;
        private int Value = 0;

        public AVFXInt( string name, int size = 4 ) : base( name ) {
            Size = size;
        }

        public int GetValue() => Value;

        public void SetValue( int value ) {
            SetAssigned( true );
            Value = value;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Size = size;
            Value = ( Size == 4 ) ? reader.ReadInt32() : reader.ReadByte();
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            if( Size == 4 ) {
                writer.Write( Value );
            }
            else {
                writer.Write( ( byte )Value );
            }
        }
    }
}
