using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;
using VFXEditor.Flatbuffer.Ephb;

namespace VfxEditor.Formats.PhybFormat.Extended {
    public class PhybEphbDelta : IUiItem {
        public readonly EphbUnknownTT Unknown1;
        public readonly EphbUnknownTT Unknown2;
        public readonly ParsedFloat Unknown3 = new( "Unknown 3" );

        public PhybEphbDelta() { }

        public PhybEphbDelta( EphbDeltaT delta ) : this() {
            Unknown1 = delta.Unknown1;
            Unknown2 = delta.Unknown2;
            Unknown3.Value = delta.Unknown3;
        }

        public void Draw() {
            Unknown3.Draw();
        }

        public EphbDeltaT Export() => new() {
            Unknown1 = Unknown1,
            Unknown2 = Unknown2,
            Unknown3 = Unknown3.Value
        };
    }
}
