using System;
using System.IO;
using System.Linq;
using VfxEditor.Parsing.Utils;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedEnum<T> : ParsedSimpleBase<T> where T : Enum {
        private readonly int Size;

        public ParsedEnum( string name, T value, int size = 4 ) : base( name, value ) {
            Size = size;
        }

        public ParsedEnum( string name, int size = 4 ) : base( name ) {
            Size = size;
            Value = ( T )( object )0;
        }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int size ) {
            var intValue = ( int )SizedNumberUtils.ReadSigned( reader, Size );
            Value = ( T )( object )intValue;
        }

        public override void Write( BinaryWriter writer ) {
            var intValue = Value == null ? -1 : ( int )( object )Value;
            SizedNumberUtils.WriteSigned( writer, intValue, Size );
        }

        protected override void DrawBody() {
            var options = ( T[] )Enum.GetValues( typeof( T ) );
            var text = options.Contains( Value ) ? Value.ToString() : $"[UNKNOWN - {( int )( object )Value}]";
            if( UiUtils.EnumComboBox( Name, text, options, Value, out var value ) ) Update( value );
        }
    }
}
