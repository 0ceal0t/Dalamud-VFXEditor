using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Parsing {
    public abstract class ParsedData {
        protected readonly List<ParsedBase> Parsed;

        public ParsedData() {
            Parsed = GetParsed();
        }

        protected abstract List<ParsedBase> GetParsed();

        protected void ReadParsed( BinaryReader reader ) {
            foreach( var parsed in Parsed ) parsed.Read( reader );
        }

        protected void WriteParsed( BinaryWriter writer ) {
            foreach( var parsed in Parsed ) parsed.Write( writer );
        }

        protected void DrawParsed( CommandManager command ) {
            foreach( var parsed in Parsed ) parsed.Draw( command );
        }
    }
}
