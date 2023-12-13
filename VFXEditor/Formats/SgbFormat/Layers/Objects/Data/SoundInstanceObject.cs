using System.IO;
using VfxEditor.Formats.SgbFormat.Layers.Objects.Data.Utils;
using VfxEditor.Parsing;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class SoundInstanceObject : SgbObject {
        private readonly ParsedString Path = new( "Path" );
        private readonly SoundEffectParam Parameters = new();

        public SoundInstanceObject( LayerEntryType type ) : base( type ) { }

        public SoundInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {
            Path.Draw();
            Parameters.Draw();
        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {
            var offset = reader.ReadUInt32();
            Path.Value = FileUtils.ReadStringOffset( startPos, reader );
            var endPos = reader.BaseStream.Position;

            reader.BaseStream.Position = startPos + offset;
            Parameters.Read( reader );

            reader.BaseStream.Position = endPos; // reset
        }
    }
}
