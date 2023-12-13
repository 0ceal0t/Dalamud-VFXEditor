using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class EnvLocationInstanceObject : SgbObject {
        public EnvLocationInstanceObject( LayerEntryType type ) : base( type ) { }

        public EnvLocationInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {

        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {

        }
    }
}
