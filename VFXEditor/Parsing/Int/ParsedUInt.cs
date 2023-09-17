using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data;

namespace VfxEditor.Parsing {
    public class ParsedUInt : ParsedSimpleBase<uint, int> {
        private int Size;

        public ParsedUInt( string name, uint value, int size = 4 ) : this( name, size ) {
            Value = value;
        }

        public ParsedUInt( string name, int size = 4 ) : base( name ) {
            Size = size;
        }

        public override void Read( BinaryReader reader ) => Read( reader, Size );

        public override void Read( BinaryReader reader, int size ) {
            Size = size;
            Value = Size switch {
                4 => reader.ReadUInt32(),
                2 => reader.ReadUInt16(),
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

            var value = ( int )Value;
            if( ImGui.InputInt( Name, ref value ) ) {
                manager.Add( new ParsedSimpleCommand<uint>( this, ( uint )value ) );
            }
        }

        protected override Dictionary<string, int> GetCopyMap( CopyManager manager ) => manager.Ints;

        protected override int ToCopy() => ( int )Value;

        protected override uint FromCopy( int val ) => ( uint )val;
    }
}
