using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Data;

namespace VfxEditor.Parsing {
    public class ParsedFloat4 : ParsedSimpleBase<Vector4, Vector4> {
        public ParsedFloat4( string name, Vector4 defaultValue ) : this( name ) {
            Value = defaultValue;
        }

        public ParsedFloat4( string name ) : base( name ) { }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int _ ) {
            Value.X = reader.ReadSingle();
            Value.Y = reader.ReadSingle();
            Value.Z = reader.ReadSingle();
            Value.W = reader.ReadSingle();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Value.X );
            writer.Write( Value.Y );
            writer.Write( Value.Z );
            writer.Write( Value.W );
        }

        public override void Draw( CommandManager manager ) {
            Copy( manager );

            var value = Value;
            if( ImGui.InputFloat4( Name, ref value ) ) {
                manager.Add( new ParsedSimpleCommand<Vector4>( this, value ) );
            }
        }

        protected override Dictionary<string, Vector4> GetCopyMap( CopyManager manager ) => manager.Vector4s;

        protected override Vector4 ToCopy() => Value;

        protected override Vector4 FromCopy( Vector4 val ) => val;
    }
}
