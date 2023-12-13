using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class TreasureInstanceObject : SgbObject {
        public TreasureInstanceObject( LayerEntryType type ) : base( type ) { }

        public TreasureInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {

        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {

        }
    }
}
