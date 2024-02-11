using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MdlFormat.Box {
    public class MdlBoundingBox : IUiItem {
        public readonly ParsedFloat4 Min = new( "Minimum" );
        public readonly ParsedFloat4 Max = new( "Maximum" );

        public MdlBoundingBox( BinaryReader reader ) {
            Min.Read( reader );
            Max.Read( reader );
        }

        public virtual void Draw() {
            Min.Draw();
            Max.Draw();
        }
    }
}
