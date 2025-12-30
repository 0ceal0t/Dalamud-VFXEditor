using VfxEditor.Flatbuffer;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.PhybFormat.Extended.Ephb {
    public class PhybEphbEta : IUiItem {
        public readonly ParsedUInt Unknown1 = new( "Unknown 1" );
        public readonly ParsedUInt Unknown2 = new( "Unknown 2" );
        public readonly ParsedUInt Unknown3 = new( "Unknown 3" );
        public readonly ParsedUInt Unknown4 = new( "Unknown 4" );

        public PhybEphbEta() { }

        public PhybEphbEta( EphbEta eta ) : this() {
            Unknown1.Value = eta.Unknown1;
            Unknown2.Value = eta.Unknown2;
            Unknown3.Value = eta.Unknown3;
            Unknown4.Value = eta.Unknown4;
        }

        public void Draw() {
            Unknown1.Draw();
            Unknown2.Draw();
            Unknown3.Draw();
            Unknown4.Draw();
        }

        public EphbEta Export() => new() {
            Unknown1 = ( ushort )Unknown1.Value,
            Unknown2 = ( ushort )Unknown2.Value,
            Unknown3 = ( ushort )Unknown3.Value,
            Unknown4 = ( ushort )Unknown4.Value,
        };
    }
}
