using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class ClientPathInstanceObject : SgbObject {
        public ClientPathInstanceObject( LayerEntryType type ) : base( type ) { }

        public ClientPathInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {

        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {

        }
    }
}
