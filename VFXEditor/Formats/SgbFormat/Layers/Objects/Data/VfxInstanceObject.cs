using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class VfxInstanceObject : SgbObject {
        public VfxInstanceObject( LayerEntryType type ) : base( type ) { }

        public VfxInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawData() {

        }

        protected override void ReadData( BinaryReader reader, long startPos ) {

        }
    }
}
