using System;
using System.IO;
using System.Linq;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedEnum<T> : ParsedSimpleBase<T> where T : Enum {
        private readonly int Size;
        public Func<ICommand> ExtraCommand; // can be changed later

        public ParsedEnum( string name, T value, Func<ICommand> extraCommand = null, int size = 4 ) : this( name, extraCommand, size ) {
            Value = value;
        }

        public ParsedEnum( string name, Func<ICommand> extraCommand = null, int size = 4 ) : base( name ) {
            Size = size;
            ExtraCommand = extraCommand;
            Value = ( T )( object )0;
        }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int size ) {
            var intValue = Size switch {
                4 => reader.ReadInt32(),
                2 => reader.ReadInt16(),
                1 => reader.ReadByte(),
                _ => reader.ReadByte()
            };
            Value = ( T )( object )intValue;
        }

        public override void Write( BinaryWriter writer ) {
            var intValue = Value == null ? -1 : ( int )( object )Value;
            if( Size == 4 ) writer.Write( intValue );
            else if( Size == 2 ) writer.Write( ( short )intValue );
            else writer.Write( ( byte )intValue );
        }

        public override void Draw( CommandManager manager ) {
            // Copy/Paste
            var copy = manager.Copy;
            if( copy.IsCopying ) copy.SetValue( this, Name, Value );
            if( copy.IsPasting && copy.GetValue<T>( this, Name, out var val ) ) {
                copy.PasteCommand.Add( new ParsedEnumCommand<T>( this, val, ExtraCommand?.Invoke() ) );
            }

            var options = ( T[] )Enum.GetValues( typeof( T ) );
            var text = options.Contains( Value ) ? Value.ToString() : "[UNKNOWN]";
            if( UiUtils.EnumComboBox( Name, text, options, Value, out var newValue ) ) {
                manager.Add( new ParsedEnumCommand<T>( this, newValue, ExtraCommand?.Invoke() ) );
            }
        }
    }
}
