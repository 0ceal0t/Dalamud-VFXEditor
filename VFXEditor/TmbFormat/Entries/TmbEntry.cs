using VfxEditor.Utils;
using VfxEditor.TmbFormat.Utils;
using System.Collections.Generic;
using VfxEditor.Parsing;

namespace VfxEditor.TmbFormat.Entries {
    public abstract class TmbEntry : TmbItemWithTime {
        public abstract string DisplayName { get; }

        private readonly List<ParsedBase> Parsed;

        public TmbEntry( bool papEmbedded ) : base( papEmbedded ) {
            Parsed = GetParsed();
        }

        public TmbEntry( TmbReader reader, bool papEmbedded ) : base( reader, papEmbedded ) {
            Parsed = GetParsed();
        }

        public abstract void Draw( string id );

        protected void ReadParsed( TmbReader reader ) {
            foreach( var item in Parsed ) item.Read( reader );
        }

        protected void WriteParsed( TmbWriter writer ) {
            foreach( var item in Parsed ) item.Write( writer );
        }

        protected void DrawParsed( string id ) {
            foreach( var item in Parsed ) item.Draw( id, Command );
        }

        protected abstract List<ParsedBase> GetParsed();
    }
}
