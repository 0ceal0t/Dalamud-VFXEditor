using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class SharedGroupInstanceObject : SgbObject {
        public SharedGroupInstanceObject( LayerEntryType type ) : base( type ) { }

        public SharedGroupInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {

        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {

        }
    }
}
