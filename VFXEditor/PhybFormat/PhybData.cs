using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.PhybFormat {
    public abstract class PhybData : IUiItem {
        protected readonly PhybFile File;
        private readonly List<ParsedBase> Parsed;

        public PhybData( PhybFile file ) {
            File = file;
            Parsed = GetParsed();
        }

        public PhybData( PhybFile file, BinaryReader reader ) : this( file ) {
            foreach( var parsed in Parsed ) parsed.Read( reader );
        }

        protected abstract List<ParsedBase> GetParsed();

        public void Write( BinaryWriter writer ) {
            foreach( var parsed in Parsed ) parsed.Write( writer );
        }

        public void Draw() {
            foreach( var parsed in Parsed ) parsed.Draw( CommandManager.Phyb );
        }
    }
}
