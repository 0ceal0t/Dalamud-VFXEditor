using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;
using VFXEditor.Flatbuffer.Ephb;

namespace VfxEditor.Formats.PhybFormat.Extended {
    public class PhybEphbEpsilon : IUiItem {
        public readonly ParsedUInt Unknown1 = new( "Unknown 1" );
        public readonly EphbUnknownTT Unknown2;
        public readonly ParsedUInt Unknown3 = new( "Unknown 3" );
        public readonly ParsedFloat Unknown4 = new( "Unknown 4" );
        public readonly ParsedFloat Unknown5 = new( "Unknown 5" );

        public PhybEphbEpsilon() { }

        public PhybEphbEpsilon( EphbEpsilonT epsilon ) : this() {
            Unknown1.Value = epsilon.Unknown1;
            Unknown2 = epsilon.Unknown2;
            Unknown3.Value = epsilon.Unknown3;
            Unknown4.Value = epsilon.Unknown4;
            Unknown5.Value = epsilon.Unknown5;
        }

        public void Draw() {
            Unknown1.Draw();
            Unknown3.Draw();
            Unknown4.Draw();
            Unknown5.Draw();
        }

        public EphbEpsilonT Export() => new() {
            Unknown1 = ( ushort )Unknown1.Value,
            Unknown2 = Unknown2,
            Unknown3 = ( ushort )Unknown3.Value,
            Unknown4 = Unknown4.Value,
            Unknown5 = Unknown5.Value,
        };
    }
}
