using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data;

namespace VfxEditor.Parsing {
    public class ParsedBool : ParsedSimpleBase<bool, bool> {
        private int Size;
        private int IntValue => Value ? 1 : 0;

        public ParsedBool( string name, bool value, int size = 4 ) : this( name, size ) {
            Value = value;
        }

        public ParsedBool( string name, int size = 4 ) : base( name ) {
            Size = size;
        }

        public override void Read( BinaryReader reader ) => Read( reader, Size );

        public override void Read( BinaryReader reader, int size ) {
            Size = size;
            Value = ( Size switch {
                4 => reader.ReadInt32(),
                2 => reader.ReadInt16(),
                1 => reader.ReadByte(),
                _ => reader.ReadByte()
            } ) == 1;
        }

        public override void Write( BinaryWriter writer ) {
            if( Size == 4 ) writer.Write( IntValue );
            else if( Size == 2 ) writer.Write( ( short )IntValue );
            else writer.Write( ( byte )IntValue );
        }

        public override void Draw( CommandManager manager ) {
            Copy( manager );

            var value = Value;
            if( ImGui.Checkbox( Name, ref value ) ) {
                manager.Add( new ParsedSimpleCommand<bool>( this, value ) );
            }
        }

        protected override Dictionary<string, bool> GetCopyMap( CopyManager manager ) => manager.Bools;

        protected override bool ToCopy() => Value;

        protected override bool FromCopy( bool val ) => val;
    }
}
