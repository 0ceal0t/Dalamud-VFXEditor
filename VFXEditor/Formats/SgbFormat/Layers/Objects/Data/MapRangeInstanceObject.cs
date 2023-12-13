using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class MapRangeInstanceObject : SgbObject {
        public MapRangeInstanceObject( LayerEntryType type ) : base( type ) { }

        public MapRangeInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {

        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {

        }
    }
}
