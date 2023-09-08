using System.IO;
using VfxEditor.Parsing;
using VfxEditor.PhybFormat.Utils;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.PhybFormat {
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

        public virtual void Draw() => DrawParsed( CommandManager.Phyb );
    }
}
