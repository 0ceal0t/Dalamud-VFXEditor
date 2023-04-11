using System;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedSimpleEnum<T> : ParsedInt where T : Enum {
        private readonly T[] Options;

        public T GetValue() => ToEnum( Value );

        public ParsedSimpleEnum( string name, T[] options, int size = 4 ) : base( name, size ) {
            Options = options;
        }

        public ParsedSimpleEnum( string name, T[] options, int defaultValue, int size = 4 ) : base( name, defaultValue, size ) {
            Options = options;
        }

        public ParsedSimpleEnum( string name, T[] options, T defaultValue, int size = 4 ) : base( name, ToInt( defaultValue ), size ) {
            Options = options;
        }

        private static T ToEnum( int value ) => ( T )( object )value;

        private static int ToInt( T value ) => ( int )( object )value;

        public override void Draw( string id, CommandManager manager ) {
            // Copy/Paste - same as regular int
            var copy = manager.Copy;
            if( copy.IsCopying ) copy.Ints[Name] = Value;
            if( copy.IsPasting && copy.Ints.TryGetValue( Name, out var val ) ) {
                copy.PasteCommand.Add( new ParsedSimpleCommand<int>( this, val ) );
            }

            var value = ToEnum( Value );
            if( UiUtils.EnumComboBox( $"{Name}{id}", value.ToString(), Options, value, out var newValue ) ) {
                manager.Add( new ParsedSimpleCommand<int>( this, ToInt( newValue ) ) );
            }
        }
    }
}
