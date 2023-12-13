using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class TargetMarkerInstanceObject : SgbObject {
        public TargetMarkerInstanceObject( LayerEntryType type ) : base( type ) { }

        public TargetMarkerInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawData() {

        }

        protected override void ReadData( BinaryReader reader, long startPos ) {

        }
    }
}
