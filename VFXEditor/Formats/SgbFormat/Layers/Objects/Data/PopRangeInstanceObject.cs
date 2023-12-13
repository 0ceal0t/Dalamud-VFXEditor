using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class PopRangeInstanceObject : SgbObject {
        public PopRangeInstanceObject( LayerEntryType type ) : base( type ) { }

        public PopRangeInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawData() {

        }

        protected override void ReadData( BinaryReader reader, long startPos ) {

        }
    }
}
