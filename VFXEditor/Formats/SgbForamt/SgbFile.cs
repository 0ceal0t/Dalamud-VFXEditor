using System.IO;
using VfxEditor.FileManager;

namespace VfxEditor.Formats.SgbForamt {
    public class SgbFile : FileManagerFile {
        public SgbFile( BinaryReader reader, bool verify ) : base() {
            reader.ReadInt32(); // magic
            reader.ReadInt32(); // total file size

            var numberOfScenes = reader.ReadInt32();

        }

        public override void Write( BinaryWriter writer ) {

        }

        public override void Draw() {

        }
    }
}
