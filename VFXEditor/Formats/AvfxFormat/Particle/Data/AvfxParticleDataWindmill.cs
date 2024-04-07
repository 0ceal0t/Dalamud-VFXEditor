using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataWindmill : AvfxDataWithParameters {
        public readonly AvfxEnum<WindmillUVType> WindmillUVType = new( "Windmill UV Type", "WUvT" );

        public AvfxParticleDataWindmill() : base() {
            Parsed = [
                WindmillUVType
            ];

            ParameterTab.Add( WindmillUVType );
        }
    }
}
