using VfxEditor.Utils;
using VfxEditor.TmbFormat.Utils;
using System.Collections.Generic;
using VfxEditor.Parsing;

namespace VfxEditor.TmbFormat.Entries {
    public abstract class TmbEntry : TmbItemWithTime {
        public abstract string DisplayName { get; }

        protected List<ParsedBase> Parsed;

        public TmbEntry() : base() { }

        public TmbEntry( TmbReader reader ) : base( reader ) { }

        public abstract void Draw( string id );

        protected void ReadParsed( TmbReader reader ) {
            foreach( var item in Parsed ) item.Read( reader );
        }

        protected void WriteParsed( TmbWriter writer ) {
            foreach( var item in Parsed ) item.Write( writer );
        }

        protected void DrawParsed( string id ) {
            foreach( var item in Parsed ) item.Draw( id, CommandManager.Tmb );
        }
    }
}
