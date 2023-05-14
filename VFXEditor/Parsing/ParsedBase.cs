using System.IO;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.Parsing {
    public abstract class ParsedBase : IParsedUiBase {
        public abstract void Draw( CommandManager manager );

        public virtual void Read( TmbReader reader ) => Read( reader.Reader );

        public abstract void Read( BinaryReader reader );

        public abstract void Read( BinaryReader reader, int size );

        public virtual void Write( TmbWriter writer ) => Write( writer.Writer );

        public abstract void Write( BinaryWriter writer );
    }
}
