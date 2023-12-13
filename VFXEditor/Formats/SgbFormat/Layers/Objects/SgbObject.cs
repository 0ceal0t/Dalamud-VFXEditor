using Dalamud.Interface.Utility.Raii;
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
        private readonly ParsedFloat3 Scale = new( "Scale" );

        public SgbObject( LayerEntryType type ) {
            Type = type;
        }

        protected void Read( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position - 4; // account for type
            Id.Read( reader );
            Dalamud.Log( $"{reader.BaseStream.Position:X8}" );
            Name.Value = FileUtils.ReadStringOffset( startPos, reader );
            Translation.Read( reader );
            Rotation.Read( reader );
            Scale.Read( reader );

            ReadBody( reader, startPos );
        }

        protected abstract void ReadBody( BinaryReader reader, long startPos );

        public void Draw() {
            Id.Draw();
            Name.Draw();
            using( var disabled = ImRaii.Disabled() ) {
                Translation.Draw();
                Rotation.Draw();
                Scale.Draw();
            }
            DrawBody();
        }

        protected abstract void DrawBody();
    }
}
