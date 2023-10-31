using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.SkpFormat.Slope {
    public class SkpSlopePoint : IUiItem {
        private readonly ParsedFloat3 Position = new( "Position" );

        public SkpSlopePoint() { }

        public SkpSlopePoint( BinaryReader reader ) => Position.Read( reader );

        public void Write( BinaryWriter writer ) => Position.Write( writer );

        public void Draw() => Position.Draw();
    }
}
