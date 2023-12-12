using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class AetheryteInstanceObject : SgbObject {
        public AetheryteInstanceObject( LayerEntryType type ) : base( type ) { }

        public AetheryteInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawData() {

        }

        protected override void ReadData( BinaryReader reader, long startPos ) {

        }
    }
}
