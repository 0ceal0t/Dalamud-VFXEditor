using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class ChairMarkerInstanceObject : SgbObject {
        public ChairMarkerInstanceObject( LayerEntryType type ) : base( type ) { }

        public ChairMarkerInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {

        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {

        }
    }
}
