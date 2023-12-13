using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class SoundInstanceObject : SgbObject {
        public SoundInstanceObject( LayerEntryType type ) : base( type ) { }

        public SoundInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {

        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {

        }
    }
}
