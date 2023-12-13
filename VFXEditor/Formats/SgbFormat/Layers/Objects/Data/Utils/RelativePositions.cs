using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data.Utils {
    public class RelativePositions {
        private readonly List<ParsedFloat3> Positions = new();
        private readonly ListView<ParsedFloat3> PositionView;

        public RelativePositions() {
            PositionView = new( Positions, () => new( "##Value" ), true );
        }

        public void Read( long startPos, BinaryReader reader ) {
            var offset = reader.ReadUInt32();
            var count = reader.ReadUInt32();
            var endPos = reader.BaseStream.Position;

            reader.BaseStream.Position = startPos + offset;
            for( var i = 0; i < count; i++ ) {
                var position = new ParsedFloat3( "##Value" );
                position.Read( reader );
                Positions.Add( position );
            }

            reader.BaseStream.Position = endPos; // reset
        }

        public void Draw() {
            PositionView.Draw();
        }
    }
}
