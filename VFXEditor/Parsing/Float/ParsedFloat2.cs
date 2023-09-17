using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Data;

namespace VfxEditor.Parsing {
    public class ParsedFloat2 : ParsedSimpleBase<Vector2, Vector2> {
        public ParsedFloat2( string name, Vector2 value ) : this( name ) {
            Value = value;
        }

        public ParsedFloat2( string name ) : base( name ) { }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int _ ) {
            Value.X = reader.ReadSingle();
            Value.Y = reader.ReadSingle();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Value.X );
            writer.Write( Value.Y );
        }

        public override void Draw( CommandManager manager ) {
            Copy( manager );

            var value = Value;
            if( ImGui.InputFloat2( Name, ref value ) ) {
                manager.Add( new ParsedSimpleCommand<Vector2>( this, value ) );
            }
        }

        protected override Dictionary<string, Vector2> GetCopyMap( CopyManager manager ) => manager.Vector2s;

        protected override Vector2 ToCopy() => Value;

        protected override Vector2 FromCopy( Vector2 val ) => val;
    }
}
