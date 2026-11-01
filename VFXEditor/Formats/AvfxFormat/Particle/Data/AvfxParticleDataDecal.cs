using VfxEditor.Formats.AvfxFormat.Base;
using VfxEditor.Formats.AvfxFormat.Components.Float;
using VfxEditor.Formats.AvfxFormat.Components.Int;

namespace VfxEditor.Formats.AvfxFormat.Particle.Data {
    public class AvfxParticleDataDecal : AvfxDataWithParameters {
        public readonly AvfxFloat ScalingScale = new( "Scaling Scale", "SS" );
        public readonly AvfxInt DDTT = new( "DDTT", "DDTT" );

        public AvfxParticleDataDecal() : base() {
            Parsed = GetFieldsInOrder<AvfxBase>();

            ParameterTab.Add( ScalingScale );
            ParameterTab.Add( DDTT );
        }
    }
}
