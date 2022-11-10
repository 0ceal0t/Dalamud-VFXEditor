using System;
using System.IO;

namespace VfxEditor.AVFXLib {
    public class AVFXEnum<T> : AVFXBase {
        private int Size;
        private T Value = ( T )( object )0;
        public readonly T[] Options = ( T[] )Enum.GetValues( typeof( T ) );

        public AVFXEnum( string name, int size = 4 ) : base( name ) {
            Size = size;
        }

        public T GetValue() => Value;

        public void SetValue( T value ) {
            SetAssigned( true );
            Value = value;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            var intValue = reader.ReadInt32();
            Value = ( T )( object )intValue;
            Size = size;
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            writer.Write( Value == null ? -1 : ( int )( object )Value );
        }
    }
}
