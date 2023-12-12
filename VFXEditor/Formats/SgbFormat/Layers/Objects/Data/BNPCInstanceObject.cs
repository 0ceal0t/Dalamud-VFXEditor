using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class BNPCInstanceObject : SgbObject {
        public BNPCInstanceObject( LayerEntryType type ) : base( type ) { }

        public BNPCInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawData() {

        }

        protected override void ReadData( BinaryReader reader, long startPos ) {

        }
    }
}