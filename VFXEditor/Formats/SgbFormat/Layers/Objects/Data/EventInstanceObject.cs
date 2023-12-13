using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class EventInstanceObject : SgbObject {
        public EventInstanceObject( LayerEntryType type ) : base( type ) { }

        public EventInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {

        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {

        }
    }
}
