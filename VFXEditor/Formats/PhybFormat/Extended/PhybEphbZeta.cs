using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;
using VFXEditor.Flatbuffer.Ephb;

namespace VfxEditor.Formats.PhybFormat.Extended {
    public class PhybEphbZeta : IUiItem {
        public readonly EphbUnknownTT Unknown1;
        public readonly ParsedFloat Unknown2 = new( "Unknown 2" );

        public PhybEphbZeta() { }

        public PhybEphbZeta( EphbZetaT zeta ) : this() {
            Unknown1 = zeta.Unknown1;
            Unknown2.Value = zeta.Unknown2;
        }

        public void Draw() {
            Unknown2.Draw();
        }

        public EphbZetaT Export() => new() {
            Unknown1 = Unknown1,
            Unknown2 = Unknown2.Value
        };
    }
}
