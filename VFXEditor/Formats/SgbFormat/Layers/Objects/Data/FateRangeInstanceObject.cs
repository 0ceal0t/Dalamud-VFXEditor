using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class FateRangeInstanceObject : SgbObject {
        public FateRangeInstanceObject( LayerEntryType type ) : base( type ) { }

        public FateRangeInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {

        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {

        }
    }
}
