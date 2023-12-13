using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class CollisionBoxInstanceObject : SgbObject {
        public CollisionBoxInstanceObject( LayerEntryType type ) : base( type ) { }

        public CollisionBoxInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawData() {

        }

        protected override void ReadData( BinaryReader reader, long startPos ) {

        }
    }
}
