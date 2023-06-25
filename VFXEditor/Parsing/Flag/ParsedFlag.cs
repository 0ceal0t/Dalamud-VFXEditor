using ImGuiNET;
using System;
using System.IO;

namespace VfxEditor.Parsing {
    public class ParsedFlag<T> : ParsedBase where T : Enum {
        public readonly string Name;
        private readonly int Size;
        public Func<ICommand> ExtraCommandGenerator; // can be changed later

        public T Value = ( T )( object )0;

        public ParsedFlag( string name, T defaultValue, Func<ICommand> extraCommandGenerator = null, int size = 4 ) : this( name, extraCommandGenerator, size ) {
            Value = defaultValue;
        }

        public ParsedFlag( string name, Func<ICommand> extraCommandGenerator = null, int size = 4 ) {
            Name = name;
            Size = size;
            ExtraCommandGenerator = extraCommandGenerator;
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
            if( copy.IsCopying ) copy.Ints[Name] = ( int )( object )Value;
            if( copy.IsPasting && copy.Ints.TryGetValue( Name, out var val ) ) {
                copy.PasteCommand.Add( new ParsedFlagCommand<T>( this, ( T )( object )val, ExtraCommandGenerator?.Invoke() ) );
            }

            var options = ( T[] )Enum.GetValues( typeof( T ) );
            foreach( var option in options ) {
                var intFlagValue = ( int )( object )option;
                if( intFlagValue == 0 ) continue;
                var hasFlag = HasFlag( option );
                if( ImGui.Checkbox( $"{option}", ref hasFlag ) ) {
                    var intValue = ( int )( object )Value;
                    if( hasFlag ) intValue |= intFlagValue;
                    else intValue &= ~intFlagValue;

                    var newValue = ( T )( object )intValue;
                    manager.Add( new ParsedFlagCommand<T>( this, newValue, ExtraCommandGenerator?.Invoke() ) );
                }
            }
        }

        public bool HasFlag( T option ) => Value.HasFlag( option );
    }
}
