using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data;

namespace VfxEditor.Parsing {
    public class ParsedInt : ParsedSimpleBase<int, int> {
        private int Size;

        public ParsedInt( string name, int defaultValue, int size = 4 ) : this( name, size ) {
            Value = defaultValue;
        }

        public ParsedInt( string name, int size = 4 ) : base( name ) {
            Size = size;
        }

        public override void Read( BinaryReader reader ) => Read( reader, Size );

        public override void Read( BinaryReader reader, int size ) {
            Size = size;
            Value = Size switch {
                4 => reader.ReadInt32(),
                2 => reader.ReadInt16(),
                1 => reader.ReadByte(),
                _ => reader.ReadByte()
            };
        }

        public override void Write( BinaryWriter writer ) {
            if( Size == 4 ) writer.Write( Value );
            else if( Size == 2 ) writer.Write( ( short )Value );
            else writer.Write( ( byte )Value );
        }

        public override void Draw( CommandManager manager ) {
            Copy( manager );

            var value = Value;
            if( ImGui.InputInt( Name, ref value ) ) {
                manager.Add( new ParsedSimpleCommand<int>( this, value ) );
            }
        }

        protected override Dictionary<string, int> GetCopyMap( CopyManager manager ) => manager.Ints;

        protected override int ToCopy() => Value;

        protected override int FromCopy( int val ) => val;
    }
}
