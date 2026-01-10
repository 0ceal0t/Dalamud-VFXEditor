using VfxEditor.Flatbuffer;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Assignable;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.PhybFormat.Extended.Ephb {
    public class PhybEphbGamma : IUiItem {
        public readonly ParsedFnvHash Bone = new( "Bone" );
        public readonly EphbEta Unknown1 = default;
        public readonly EphbUnknown Unknown2 = default;
        public readonly ParsedUInt Unknown3 = new( "Unknown 3" );
        public readonly ParsedUInt Unknown4 = new( "Unknown 4" );
        public readonly EphbEta Eta = default;
        public readonly AssignableData<PhybEphbDelta> Delta = new( "Delta" );

        public PhybEphbGamma() { }

        public PhybEphbGamma( EphbGamma gamma ) : this() {
            Bone.Value = ("", gamma.Bone);
            Unknown1 = gamma.A;
            Unknown2 = gamma.B;
            Unknown3.Value = gamma.C;
            Unknown4.Value = gamma.D;
            Eta = gamma.E;
            Delta.SetValue( gamma.F == null ? null : new( gamma.F ) );
        }

        public void Draw() {
            Bone.Draw();
            Delta.Draw();
            //Eta.Draw();
        }

        public EphbGamma Export() => new() {
            Bone = Bone.Hash,
            A = Unknown1,
            B = Unknown2,
            C = Unknown3.Value,
            D = Unknown4.Value,
            //E = Eta.GetValue()?.Export(),
            E = Eta,
            F = Delta.GetValue()?.Export()
        };
    }
}
