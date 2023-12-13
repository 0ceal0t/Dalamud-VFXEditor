using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data.Utils {
    public class ColorHDRI : IUiItem {
        private readonly ParsedIntColor Color = new( "Color" );
        private readonly ParsedFloat Intensity = new( "Intensity" );

        public ColorHDRI() { }

        public void Read( BinaryReader reader ) {
            Color.Read( reader );
            Intensity.Read( reader );
        }

        public void Draw() {
            Color.Draw();
            Intensity.Draw();
        }
    }
}
