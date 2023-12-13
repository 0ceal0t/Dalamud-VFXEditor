using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class GatheringInstanceObject : SgbObject {
        public GatheringInstanceObject( LayerEntryType type ) : base( type ) { }

        public GatheringInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {

        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {

        }
    }
}
