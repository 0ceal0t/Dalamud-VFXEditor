using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Interfaces;

namespace VFXEditor.Formats.KdbFormat.UnknownE {
    public class UnknownEItem : IUiItem {
        public readonly ParsedUIntHex Unknown1 = new( "Unknown 1" );
        public readonly ParsedUIntHex Unknown2 = new( "Unknown 2" );
        public readonly ParsedInt Unknown3 = new( "Unknown 3" );
        public readonly ParsedInt Unknown4 = new( "Unknown 4" );
        public readonly ParsedInt Unknown5 = new( "Unknown 5" );

        public UnknownEItem() { }

        public UnknownEItem( BinaryReader reader ) : this() {
            Unknown1.Read( reader );
            Unknown2.Read( reader );
            Unknown3.Read( reader );
            Unknown4.Read( reader );
            Unknown5.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Unknown1.Write( writer );
            Unknown2.Write( writer );
            Unknown3.Write( writer );
            Unknown4.Write( writer );
            Unknown5.Write( writer );
        }

        public void Draw() {
            Unknown1.Draw();
            Unknown2.Draw();
            Unknown3.Draw();
            Unknown4.Draw();
            Unknown5.Draw();
        }
    }
}
