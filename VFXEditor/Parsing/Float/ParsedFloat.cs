using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data;

namespace VfxEditor.Parsing {
    public class ParsedFloat : ParsedSimpleBase<float, float> {
        public ParsedFloat( string name, float value ) : this( name ) {
            Value = value;
        }

        public ParsedFloat( string name ) : base( name ) { }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int _ ) {
            Value = reader.ReadSingle();
        }

        public override void Write( BinaryWriter writer ) => writer.Write( Value );

        public override void Draw( CommandManager manager ) {
            Copy( manager );

            var value = Value;
            if( ImGui.InputFloat( Name, ref value ) ) {
                manager.Add( new ParsedSimpleCommand<float>( this, value ) );
            }
        }

        protected override Dictionary<string, float> GetCopyMap( CopyManager manager ) => manager.Floats;

        protected override float ToCopy() => Value;

        protected override float FromCopy( float val ) => val;
    }
}
