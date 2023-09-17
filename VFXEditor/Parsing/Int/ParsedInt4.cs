using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data;
using Int4 = SharpDX.Int4;

namespace VfxEditor.Parsing {
    public class ParsedInt4 : ParsedSimpleBase<Int4, Int4> {
        public ParsedInt4( string name, Int4 value ) : this( name ) {
            Value = value;
        }

        public ParsedInt4( string name ) : base( name ) { }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int _ ) {
            Value.X = reader.ReadInt32();
            Value.Y = reader.ReadInt32();
            Value.Z = reader.ReadInt32();
            Value.W = reader.ReadInt32();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Value.X );
            writer.Write( Value.Y );
            writer.Write( Value.Z );
            writer.Write( Value.W );
        }

        public override void Draw( CommandManager manager ) {
            Copy( manager );

            var value = Value.ToArray();
            if( ImGui.InputInt4( Name, ref value[0] ) ) {
                manager.Add( new ParsedSimpleCommand<Int4>( this, value ) );
            }
        }

        protected override Dictionary<string, Int4> GetCopyMap( CopyManager manager ) => manager.Int4s;

        protected override Int4 ToCopy() => Value;

        protected override Int4 FromCopy( Int4 val ) => val;
    }
}