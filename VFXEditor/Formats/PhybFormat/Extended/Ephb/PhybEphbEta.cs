using System.Numerics;
using VfxEditor.Flatbuffer;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;
using VFXEditor.Parsing;

namespace VfxEditor.Formats.PhybFormat.Extended.Ephb {
    public class PhybEphbEta : IUiItem {
        public readonly ParsedUInt Unknown1 = new( "Unknown 1" );
        public readonly AssignableParsed<Vector3> Unknown2 = new( new ParsedFloat3( "Unknown 2" ) );

        public PhybEphbEta() { }

        public PhybEphbEta( EphbEta eta ) : this() {
            Unknown1.Value = eta.Unknown1;
            if( eta.Unknown2 != null ) Unknown2.Value = new( eta.Unknown2.X, eta.Unknown2.Y, eta.Unknown2.Z );
        }

        public void Draw() {
            Unknown1.Draw();
            Unknown2.Draw();
        }

        public EphbEta Export() => new() {
            Unknown1 = Unknown1.Value,
            Unknown2 = Unknown2.Assigned ? new() {
                X = Unknown2.Value.X,
                Y = Unknown2.Value.Y,
                Z = Unknown2.Value.Z
            } : null
        };
    }
}
