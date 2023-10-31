using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Data;

namespace VfxEditor.UldFormat {
    public abstract class UldGenericData : IData {
        protected List<ParsedBase> Parsed = new();

        public virtual void Read( BinaryReader reader ) {
            foreach( var parsed in Parsed ) parsed.Read( reader );
        }

        public virtual void Write( BinaryWriter writer ) {
            foreach( var parsed in Parsed ) parsed.Write( writer );
        }

        public virtual void Draw() {
            foreach( var parsed in Parsed ) parsed.Draw();
        }

        protected void AddUnknown( int count, string prefix ) {
            for( var i = 1; i <= count + 1; i++ ) Parsed.Add( new ParsedUInt( $"{prefix} {i}" ) );
        }

        public void Enable() { }

        public void Disable() { }
    }
}
