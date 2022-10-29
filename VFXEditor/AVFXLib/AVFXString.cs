using System.IO;
using System.Text;

namespace VfxEditor.AVFXLib {
    public class AVFXString : AVFXBase {
        private int Size;
        private readonly int FixedSize;
        private string Value = "";

        public AVFXString( string name, int size = 4, int fixedSize = -1 ) : base( name ) {
            Size = size;
            FixedSize = fixedSize;
        }

        public string GetValue() => Value;

        public void SetValue( string value ) {
            SetAssigned( true );
            Value = value;
            Size = ( FixedSize == -1 ) ? Value.Length : FixedSize;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Size = ( FixedSize == -1 ) ? Value.Length : FixedSize;
            Value = Encoding.ASCII.GetString( reader.ReadBytes( size ) );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            var bytes = Encoding.ASCII.GetBytes( Value );
            writer.Write( bytes );
            if( FixedSize != -1 ) {
                WritePad( writer, FixedSize - bytes.Length );
            }
        }
    }
}
