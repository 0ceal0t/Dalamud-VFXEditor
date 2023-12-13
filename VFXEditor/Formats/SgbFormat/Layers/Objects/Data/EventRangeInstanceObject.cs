using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class EventRangeInstanceObject : SgbObject {
        public EventRangeInstanceObject( LayerEntryType type ) : base( type ) { }

        public EventRangeInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawData() {

        }

        protected override void ReadData( BinaryReader reader, long startPos ) {

        }
    }
}
