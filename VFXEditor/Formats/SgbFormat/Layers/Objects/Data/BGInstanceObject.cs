using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class BGInstanceObject : SgbObject {
        public BGInstanceObject( LayerEntryType type ) : base( type ) { }

        public BGInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawData() {

        }

        protected override void ReadData( BinaryReader reader, long startPos ) {

        }
    }
}
