using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class PositionMarkerInstanceObject : SgbObject {
        public PositionMarkerInstanceObject( LayerEntryType type ) : base( type ) { }

        public PositionMarkerInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {

        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {

        }
    }
}
