using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.MdlFormat.Box {
    public class MdlBoneBoundingBox : MdlBoundingBox {
        public readonly ParsedString Name = new( "Name" );

        public MdlBoneBoundingBox( string name, BinaryReader reader ) : base( reader ) {
            Name.Value = name;
            // Don't read the name
        }

        public override void Draw() {
            Name.Draw();
            base.Draw();
        }
    }
}
