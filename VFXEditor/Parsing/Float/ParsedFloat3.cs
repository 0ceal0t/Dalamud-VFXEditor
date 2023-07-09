using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Data;

namespace VfxEditor.Parsing {
    public class ParsedFloat3 : ParsedSimpleBase<Vector3, Vector3> {
        public ParsedFloat3( string name, Vector3 defaultValue ) : this( name ) {
            Value = defaultValue;
        }

        public ParsedFloat3( string name ) : base( name ) { }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int _ ) {
            Value.X = reader.ReadSingle();
            Value.Y = reader.ReadSingle();
            Value.Z = reader.ReadSingle();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Value.X );
            writer.Write( Value.Y );
            writer.Write( Value.Z );
        }

        public override void Draw( CommandManager manager ) {
            Copy( manager );

            var value = Value;
            if( ImGui.InputFloat3( Name, ref value ) ) {
                manager.Add( new ParsedSimpleCommand<Vector3>( this, value ) );
            }
        }

        protected override Dictionary<string, Vector3> GetCopyMap( CopyManager manager ) => manager.Vector3s;

        protected override Vector3 ToCopy() => Value;

        protected override Vector3 FromCopy( Vector3 val ) => val;
    }
}
