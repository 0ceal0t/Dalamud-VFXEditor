using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;
using VFXEditor.Flatbuffer.Ephb;

namespace VfxEditor.Formats.PhybFormat.Extended {
    public class PhybEphbTheta : IUiItem {
        public readonly ParsedBool Unknown1 = new( "Unknown 1" );
        public readonly ParsedFloat Unknown2 = new( "Unknown 2" );

        public PhybEphbTheta() { }

        public PhybEphbTheta( EphbThetaT theta ) : this() {
            Unknown1.Value = theta.Unknown1;
            Unknown2.Value = theta.Unknown2;
        }

        public void Draw() {
            Unknown1.Draw();
            Unknown2.Draw();
        }

        public EphbThetaT Export() => new() {
            Unknown1 = Unknown1.Value,
            Unknown2 = Unknown2.Value,
        };
    }
}
