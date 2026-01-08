using VfxEditor.Flatbuffer;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.PhybFormat.Extended.Ephb {
    public class PhybEphbZeta : IUiItem {
        public readonly ParsedUInt Unknown1 = new( "Unknown 1" );
        public readonly ParsedFloat Unknown2 = new( "Unknown 2" );
        public readonly ParsedFloat Unknown3 = new( "Unknown 3" );

        public PhybEphbZeta() { }

        public PhybEphbZeta( EphbZeta zeta ) : this() {
            Unknown1.Value = zeta.Unknown1;
            Unknown2.Value = zeta.Unknown2;
            Unknown3.Value = zeta.Unknown3;
        }

        public void Draw() {
            Unknown1.Draw();
            Unknown2.Draw();
            Unknown3.Draw();
        }

        public EphbZeta Export() => new() {
            Unknown1 = Unknown1.Value,
            Unknown2 = Unknown2.Value,
            Unknown3 = Unknown3.Value
        };
    }
}
