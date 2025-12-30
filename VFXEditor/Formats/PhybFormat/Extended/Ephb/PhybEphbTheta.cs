using VfxEditor.Flatbuffer;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.PhybFormat.Extended.Ephb {
    public class PhybEphbTheta : IUiItem {
        public readonly ParsedBool Unknown1 = new( "Unknown 1" );
        public readonly ParsedFloat Unknown2 = new( "Unknown 2" );

        public PhybEphbTheta() { }

        public PhybEphbTheta( EphbTheta theta ) : this() {
            Unknown1.Value = theta.Unknown1;
            Unknown2.Value = theta.Unknown2;
        }

        public void Draw() {
            Unknown1.Draw();
            Unknown2.Draw();
        }

        public EphbTheta Export() => new() {
            Unknown1 = Unknown1.Value,
            Unknown2 = Unknown2.Value,
        };
    }
}
