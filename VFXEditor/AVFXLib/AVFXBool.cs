using System.IO;

namespace VFXEditor.AVFXLib {
    public class AVFXBool : AVFXBase {
        private int Size;
        private bool? Value = false;

        public AVFXBool( string name, int size = 4 ) : base( name ) {
            Size = size;
        }

        public bool? GetValue() => Value;

        public void SetValue( bool? value ) {
            SetAssigned( true );
            Value = value;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            var b = reader.ReadByte();
            Value = b switch {
                0x00 => false,
                0x01 => true,
                0xff => null,
                _ => null
            };
            Size = size;
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            byte v = Value switch {
                true => 0x01,
                false => 0x00,
                null => 0xff
            };
            writer.Write( v );
            WritePad( writer, Size - 1 );
        }
    }
}
