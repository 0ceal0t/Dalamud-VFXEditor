using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Assignable;
using VfxEditor.Ui.Interfaces;
using VFXEditor.Flatbuffer.Ephb;

namespace VfxEditor.Formats.PhybFormat.Extended.Ephb {
    public class PhybEphbGamma : IUiItem {
        public readonly ParsedFnvHash Bone = new( "Bone" );
        public readonly AssignableData<PhybEphbIota> Iota = new( "Iota" );
        private readonly EphbUnknownTT Unknown1;
        private readonly EphbUnknownTT Unknown2;
        public readonly ParsedFloat Unknown3 = new( "Unknown 3" );
        public readonly AssignableData<PhybEphbTheta> Theta = new( "Theta" );
        public readonly AssignableData<PhybEphbDelta> Delta = new( "Delta" );

        public PhybEphbGamma() { }

        public PhybEphbGamma( EphbGammaT gamma ) : this() {
            Bone.Value = ("", gamma.Bone);
            Iota.SetValue( gamma.Iota == null ? null : new( gamma.Iota ) );
            Unknown1 = gamma.Unknown1;
            Unknown2 = gamma.Unknown2;
            Unknown3.Value = gamma.Unknown3;
            Theta.SetValue( gamma.Theta == null ? null : new( gamma.Theta ) );
            Delta.SetValue( gamma.Delta == null ? null : new( gamma.Delta ) );
        }

        public void Draw() {
            Bone.Draw();
            Unknown3.Draw();
            Iota.Draw();
            Theta.Draw();
            Delta.Draw();
        }

        public EphbGammaT Export() => new() {
            Bone = Bone.Hash,
            Iota = Iota.GetValue()?.Export(),
            Unknown1 = Unknown1,
            Unknown2 = Unknown2,
            Unknown3 = Unknown3.Value,
            Theta = Theta.GetValue()?.Export(),
            Delta = Delta.GetValue()?.Export()
        };
    }
}
