using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class AetheryteInstanceObject : SgbObject {
        public AetheryteInstanceObject( LayerEntryType type ) : base( type ) { }

        public AetheryteInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {

        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {

        }
    }
}
