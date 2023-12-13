using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class FateRangeInstanceObject : SgbObject {
        public FateRangeInstanceObject( LayerEntryType type ) : base( type ) { }

        public FateRangeInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawData() {

        }

        protected override void ReadData( BinaryReader reader, long startPos ) {

        }
    }
}
