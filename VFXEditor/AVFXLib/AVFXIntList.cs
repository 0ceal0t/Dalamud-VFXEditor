using System.Collections.Generic;
using System.IO;

namespace VFXEditor.AVFXLib {
    public class AVFXIntList : AVFXBase {
        private int Size;
        private readonly List<int> Value = new() { 0 };

        public AVFXIntList( string name, int size = 1 ) : base( name ) {
            Size = size;
        }

        public List<int> GetValue() => Value;

        public void SetValue( List<int> value ) {
            SetAssigned( true );
            Value.Clear();
            Value.AddRange( value );
            Size = Value.Count;
        }

        public void SetValue( int value ) {
            SetValue( new List<int> { value } );
        }

        public void SetValue( int value, int idx ) {
            SetAssigned( true );
            Value[idx] = value;
        }

        public void AddItem( int item ) {
            SetAssigned( true );
            Size++;
            Value.Add( item );
        }

        public void RemoveItem( int idx ) {
            SetAssigned( true );
            Size--;
            Value.Remove( idx );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Size = size;
            Value.Clear();
            for( var i = 0; i < Size; i++ ) {
                Value.Add( reader.ReadByte() );
            }
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            foreach( var item in Value ) writer.Write( ( byte )item );
        }
    }
}
