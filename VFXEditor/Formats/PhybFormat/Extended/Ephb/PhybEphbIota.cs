using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;
using VFXEditor.Flatbuffer.Ephb;

namespace VfxEditor.Formats.PhybFormat.Extended.Ephb {
    public class PhybEphbIota : IUiItem {
        private readonly EphbUnknownTT Unknown1;
        private readonly EphbUnknownTT Unknown2;
        private readonly EphbUnknownTT Unknown3;
        private readonly ParsedFloat Unknown4 = new( "Unknown 4" );

        public PhybEphbIota() { }

        public PhybEphbIota( EphbIotaT iota ) : this() {
            Unknown1 = iota.Unknown1;
            Unknown2 = iota.Unknown2;
            Unknown3 = iota.Unknown3;
            Unknown4.Value = iota.Unknown4;
        }

        public void Draw() {
            Unknown4.Draw();
        }

        public EphbIotaT Export() => new() {
            Unknown1 = Unknown1,
            Unknown2 = Unknown2,
            Unknown3 = Unknown3,
            Unknown4 = Unknown4.Value,
        };
    }
}
