using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data.Utils {
    public class PathControlPoint : IUiItem {
        private readonly ParsedFloat3 Translation = new( "Translation" );
        private readonly ParsedShort PointId = new( "Point Id" );
        private readonly ParsedByte Select = new( "Select" );

        public PathControlPoint() { }

        public PathControlPoint( BinaryReader reader ) {
            Translation.Read( reader );
            PointId.Read( reader );
            Select.Read( reader );
            reader.ReadByte(); // padding
        }

        public void Draw() {
            Translation.Draw();
            PointId.Draw();
            Select.Draw();
        }
    }
}
