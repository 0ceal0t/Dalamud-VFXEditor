using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Assignable;
using VfxEditor.Ui.Interfaces;
using VFXEditor.Flatbuffer.Ephb;

namespace VfxEditor.Formats.PhybFormat.Extended {
    public class PhybEphbGamma : IUiItem {
        public readonly ParsedFnvHash Bone = new( "Bone" );
        private readonly EphbUnknownTT Unknown1;
        private readonly EphbUnknownTT Unknown2;
        private readonly EphbUnknownTT Unknown3;
        public readonly ParsedFloat Unknown4 = new( "Unknown 4" );
        public readonly AssignableData<PhybEphbTheta> Theta = new( "Theta" );
        public readonly AssignableData<PhybEphbDelta> Delta = new( "Delta" );

        public PhybEphbGamma() { }

        public PhybEphbGamma( EphbGammaT gamma ) : this() {
            Bone.Value = ("", gamma.Bone);
            Unknown1 = gamma.Unknown1;
            Unknown2 = gamma.Unknown2;
            Unknown3 = gamma.Unknown3;
            Unknown4.Value = gamma.Unknown4;
            Theta.SetValue( gamma.Theta == null ? null : new( gamma.Theta ) );
            Delta.SetValue( gamma.Delta == null ? null : new( gamma.Delta ) );
        }

        public void Draw() {
            Bone.Draw();
            Unknown4.Draw();
            Theta.Draw();
            Delta.Draw();
        }

        public EphbGammaT Export() => new() {
            Bone = Bone.Hash,
            Unknown1 = Unknown1,
            Unknown2 = Unknown2,
            Unknown3 = Unknown3,
            Unknown4 = Unknown4.Value,
            Theta = Theta.GetValue()?.Export(),
            Delta = Delta.GetValue()?.Export()
        };
    }
}
