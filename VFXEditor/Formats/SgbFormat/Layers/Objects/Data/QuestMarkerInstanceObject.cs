using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class QuestMarkerInstanceObject : SgbObject {
        public QuestMarkerInstanceObject( LayerEntryType type ) : base( type ) { }

        public QuestMarkerInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawData() {

        }

        protected override void ReadData( BinaryReader reader, long startPos ) {

        }
    }
}
