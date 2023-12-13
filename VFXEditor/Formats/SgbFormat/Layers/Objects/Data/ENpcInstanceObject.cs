using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class ENpcInstanceObject : SgbObject {
        public ENpcInstanceObject( LayerEntryType type ) : base( type ) { }

        public ENpcInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {

        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {

        }
    }
}
