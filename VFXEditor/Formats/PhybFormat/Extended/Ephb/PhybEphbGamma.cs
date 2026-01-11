using VfxEditor.Flatbuffer;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Assignable;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.PhybFormat.Extended.Ephb {
    public class PhybEphbGamma : IUiItem {
        public readonly ParsedFnvHash Bone = new( "Bone" );
        public readonly AssignableData<PhybEphbEta> Eta1 = new( "Eta 1" );
        public readonly EphbUnknown? Unknown2 = default;
        public readonly ParsedUInt Unknown3 = new( "Unknown 3" );
        public readonly ParsedUInt Unknown4 = new( "Unknown 4" );
        public readonly AssignableData<PhybEphbEta> Eta2 = new( "Eta 2" );
        public readonly AssignableData<PhybEphbDelta> Delta = new( "Delta" );

        public PhybEphbGamma() { }

        public PhybEphbGamma( EphbGamma gamma ) : this() {
            Bone.Value = ("", gamma.Bone);
            Eta1.SetValue( gamma.A == null ? null : new( gamma.A ) );
            Unknown2 = gamma.B;
            Unknown3.Value = gamma.C;
            Unknown4.Value = gamma.D;
            Eta2.SetValue( gamma.E == null ? null : new( gamma.E ) );
            Delta.SetValue( gamma.F == null ? null : new( gamma.F ) );
        }

        public void Draw() {
            Bone.Draw();
            Eta1.Draw();
            Delta.Draw();
            Eta2.Draw();
        }

        public EphbGamma Export() => new() {
            Bone = Bone.Hash,
            A = Eta1.GetValue()?.Export(),
            B = Unknown2,
            C = Unknown3.Value,
            D = Unknown4.Value,
            E = Eta2.GetValue()?.Export(),
            F = Delta.GetValue()?.Export()
        };
    }
}
