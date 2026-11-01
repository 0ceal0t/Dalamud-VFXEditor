using System.IO;
using VfxEditor.Formats.PhybFormat.Utils;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.PhybFormat {
    public abstract class PhybData : ParsedData, IUiItem {
        protected readonly PhybFile File;

        public PhybData( PhybFile file ) : base() {
            File = file;
        }

        public PhybData( PhybFile file, BinaryReader reader ) : this( file ) {
            ReadParsed( reader );
        }

        public virtual void Write( SimulationWriter writer ) {
            foreach( var parsed in Parsed ) parsed.Write( writer );
        }

        public virtual void Write( BinaryWriter writer ) => WriteParsed( writer );

        public virtual void Draw() => DrawParsed();
    }
}
