using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class VFXInstanceObject : SgbObject {
        public VFXInstanceObject( LayerEntryType type ) : base( type ) { }

        public VFXInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawData() {

        }

        protected override void ReadData( BinaryReader reader, long startPos ) {

        }
    }
}
