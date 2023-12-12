using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects {
    public abstract class SgbObject : IUiItem {
        public readonly LayerEntryType Type;
        public readonly ParsedUInt Id = new( "Id" );
        public readonly ParsedString Name = new( "Name" );
        private readonly ParsedFloat3 Translation = new( "Translation" );
        private readonly ParsedFloat3 Rotation = new( "Rotation" );
        private readonly ParsedFloat3 Scale = new( "Scale " );

        public SgbObject( LayerEntryType type ) {
            Type = type;
        }

        protected void Read( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position - 4; // account for type
            Id.Read( reader );
            Name.Value = FileUtils.ReadStringOffset( startPos, reader );
            Translation.Read( reader );
            Rotation.Read( reader );
            Scale.Read( reader );

            ReadData( reader, startPos );
        }

        protected abstract void ReadData( BinaryReader reader, long startPos );

        public void Draw() {
            Id.Draw();
            Name.Draw();
            Translation.Draw();
            Rotation.Draw();
            Scale.Draw();
            DrawData();
        }

        protected abstract void DrawData();
    }
}
