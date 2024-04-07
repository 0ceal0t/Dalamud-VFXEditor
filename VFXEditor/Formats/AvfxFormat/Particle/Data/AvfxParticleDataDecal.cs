namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataDecal : AvfxDataWithParameters {
        public readonly AvfxFloat ScalingScale = new( "Scaling Scale", "SS" );

        public AvfxParticleDataDecal() : base() {
            Parsed = [
                ScalingScale
            ];

            ParameterTab.Add( ScalingScale );
        }
    }
}
