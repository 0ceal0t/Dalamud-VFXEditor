using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Data;

namespace VfxEditor.Parsing.Int {
    public class ParsedIntByte4 : ParsedSimpleBase<int, int> {
        public ParsedIntByte4( string name ) : base( name ) { }

        public override void Draw( CommandManager manager ) {
            Copy( manager );

            var bytes = BitConverter.GetBytes( Value );
            var value = bytes.Select( x => ( int )x ).ToArray();

            if( ImGui.InputInt4( Name, ref value[0] ) ) {
                var newValue = BitConverter.ToInt32( value.Select( x => ( byte )x ).ToArray() );
                manager.Add( new ParsedSimpleCommand<int>( this, newValue ) );
            }
        }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int size ) {
            Value = reader.ReadInt32();
        }

        public override void Write( BinaryWriter writer ) => writer.Write( Value );

        protected override int FromCopy( int val ) => val;

        protected override Dictionary<string, int> GetCopyMap( CopyManager manager ) => manager.Ints;

        protected override int ToCopy() => Value;
    }
}
