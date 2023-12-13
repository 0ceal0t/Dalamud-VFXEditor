using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class LineVfxInstanceObject : SgbObject {
        public LineVfxInstanceObject( LayerEntryType type ) : base( type ) { }

        public LineVfxInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawData() {

        }

        protected override void ReadData( BinaryReader reader, long startPos ) {

        }
    }
}
