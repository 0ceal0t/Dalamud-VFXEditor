using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class ENPCInstanceObject : SgbObject {
        public ENPCInstanceObject( LayerEntryType type ) : base( type ) { }

        public ENPCInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {

        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {

        }
    }
}
