using System.IO;

namespace VFXEditor.AVFXLib {
    public class AVFXFloat : AVFXBase {
        private int Size;
        private float Value = 0.0f;

        public AVFXFloat( string name, int size = 4 ) : base( name ) {
            Size = size;
        }

        public float GetValue() => Value;

        public void SetValue( float value ) {
            SetAssigned( true );
            Value = value;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Value = reader.ReadSingle();
            Size = size;
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            writer.Write( Value );
        }
    }
}
