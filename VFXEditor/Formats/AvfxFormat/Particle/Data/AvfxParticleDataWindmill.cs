using VfxEditor.Formats.AvfxFormat.Base;
using VfxEditor.Formats.AvfxFormat.Components.Enums;
using static VfxEditor.Formats.AvfxFormat.Base.Enums;

namespace VfxEditor.Formats.AvfxFormat.Particle.Data {
    public class AvfxParticleDataWindmill : AvfxDataWithParameters {
        public readonly AvfxEnum<WindmillUVType> WindmillUVType = new( "Windmill UV Type", "WUvT" );

        public AvfxParticleDataWindmill() : base() {
            Parsed = GetFieldsInOrder<AvfxBase>( typeof( AvfxParticleDataWindmill ) );

            ParameterTab.Add( WindmillUVType );
        }
    }
}
