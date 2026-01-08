using VfxEditor.Flatbuffer;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.PhybFormat.Extended.Ephb {
    public class PhybEphbDelta : IUiItem {
        public readonly ParsedUInt Unknown1 = new( "Unknown 1" );
        public readonly ParsedFloat Unknown2 = new( "Unknown 2" );
        public readonly ParsedFloat3 Unknown3 = new( "Unknown 3" );

        public PhybEphbDelta() { }

        public PhybEphbDelta( EphbDelta delta ) : this() {
            Unknown1.Value = delta.Unknown1;
            Unknown2.Value = delta.Unknown2;
            Unknown3.Value = new(delta.Unknown3.X, delta.Unknown3.Y, delta.Unknown3.Z );
        }

        public void Draw() {
            Unknown1.Draw();
            Unknown2.Draw();
            Unknown3.Draw();
        }

        public EphbDelta Export() => new() {
            Unknown1 = Unknown1.Value,
            Unknown2 = Unknown2.Value,
            Unknown3 = new() {
                X = Unknown3.Value.X,
                Y = Unknown3.Value.Y,
                Z = Unknown3.Value.Z
            }
        };
    }
}
