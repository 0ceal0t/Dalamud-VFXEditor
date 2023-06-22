using System.IO;
using VfxEditor.Parsing.Utils;

namespace VfxEditor.Parsing {
    public abstract class ParsedBase : IParsedUiBase {
        public abstract void Draw( CommandManager manager );

        public virtual void Read( ParsingReader reader ) => Read( reader.Reader );

        public abstract void Read( BinaryReader reader );

        public abstract void Read( BinaryReader reader, int size );

        public virtual void Write( ParsingWriter writer ) => Write( writer.Writer );

        public abstract void Write( BinaryWriter writer );
    }
}
