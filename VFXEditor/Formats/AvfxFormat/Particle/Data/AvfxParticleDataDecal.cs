namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataDecal : AvfxDataWithParameters {
        public readonly AvfxFloat ScalingScale = new( "Scaling Scale", "SS" );
        public readonly AvfxInt DDTT = new( "DDTT", "DDTT" );

        public AvfxParticleDataDecal() : base() {
            Parsed = [
                ScalingScale,
                DDTT,
            ];

            ParameterTab.Add( ScalingScale );
            ParameterTab.Add( DDTT );
        }
    }
}
