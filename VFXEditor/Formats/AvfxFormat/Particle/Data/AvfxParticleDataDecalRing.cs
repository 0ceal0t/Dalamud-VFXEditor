namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataDecalRing : AvfxDataWithParameters {
        public readonly AvfxCurve Width = new( "Width", "WID" );
        public readonly AvfxFloat ScalingScale = new( "Scaling Scale", "SS" );
        public readonly AvfxFloat RingFan = new( "Ring Fan", "RF" );

        public AvfxParticleDataDecalRing() : base() {
            Parsed = [
                Width,
                ScalingScale,
                RingFan
            ];

            ParameterTab.Add( ScalingScale );
            ParameterTab.Add( RingFan );

            DisplayTabs.Add( Width );
        }
    }
}
