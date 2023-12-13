using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class TriggerBoxInstanceObject : SgbObject {
        private readonly ParsedEnum<TriggerBoxShape> Shape = new( "Shape" );
        private readonly ParsedShort Priority = new( "Priority" );
        private readonly ParsedByteBool Enabled = new( "Enabled" );

        public TriggerBoxInstanceObject( LayerEntryType type ) : base( type ) { }

        public TriggerBoxInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {
            Shape.Draw();
            Priority.Draw();
            Enabled.Draw();
        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {
            Shape.Read( reader );
            Priority.Read( reader );
            Enabled.Read( reader );
            reader.ReadBytes( 5 ); // padding
        }
    }
}
