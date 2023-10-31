using System.IO;
using VfxEditor.Parsing.Utils;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Parsing {
    public abstract class ParsedBase : IUiItem {
        public abstract void Draw();

        public virtual void Read( ParsingReader reader ) => Read( reader.Reader );

        public abstract void Read( BinaryReader reader );

        public abstract void Read( BinaryReader reader, int size );

        public virtual void Write( ParsingWriter writer ) => Write( writer.Writer );

        public abstract void Write( BinaryWriter writer );
    }
}
