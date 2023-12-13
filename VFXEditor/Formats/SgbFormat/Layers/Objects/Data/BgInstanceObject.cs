using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class BgInstanceObject : SgbObject {
        public BgInstanceObject( LayerEntryType type ) : base( type ) { }

        public BgInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {

        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {

        }
    }
}
