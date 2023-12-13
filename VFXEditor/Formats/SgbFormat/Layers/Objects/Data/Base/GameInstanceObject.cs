using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class GameInstanceObject : SgbObject {
        private readonly ParsedUInt BaseId = new( "Base Id" );

        public GameInstanceObject( LayerEntryType type ) : base( type ) { }

        public GameInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {
            BaseId.Draw();
        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {
            BaseId.Read( reader );
        }
    }
}
