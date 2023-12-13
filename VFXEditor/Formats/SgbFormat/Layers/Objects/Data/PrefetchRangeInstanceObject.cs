using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class PrefetchRangeInstanceObject : SgbObject {
        public PrefetchRangeInstanceObject( LayerEntryType type ) : base( type ) { }

        public PrefetchRangeInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawData() {

        }

        protected override void ReadData( BinaryReader reader, long startPos ) {

        }
    }
}
