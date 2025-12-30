using VfxEditor.Flatbuffer;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.PhybFormat.Extended.Ephb {
    public class PhybEphbDelta : IUiItem {
        public readonly EphbUnknownT Unknown1;
        public readonly EphbUnknownT Unknown2;
        public readonly ParsedFloat Unknown3 = new( "Unknown 3" );

        public PhybEphbDelta() { }

        public PhybEphbDelta( EphbDelta delta ) : this() {
            Unknown1 = delta.Unknown1;
            Unknown2 = delta.Unknown2;
            Unknown3.Value = delta.Unknown3;
        }

        public void Draw() {
            Unknown3.Draw();
        }

        public EphbDelta Export() => new() {
            Unknown1 = Unknown1,
            Unknown2 = Unknown2,
            Unknown3 = Unknown3.Value
        };
    }
}
