namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataDecalRing : AvfxData {
        public readonly AvfxCurve Width = new( "Width", "WID" );
        public readonly AvfxFloat ScalingScale = new( "Scaling Scale", "SS" );
        public readonly AvfxFloat RingFan = new( "Ring Fan", "RF" );

        public readonly UiDisplayList Display;

        public AvfxParticleDataDecalRing() : base() {
            Parsed = new() {
                Width,
                ScalingScale,
                RingFan
            };

            DisplayTabs.Add( Display = new UiDisplayList( "Parameters" ) );
            Display.Add( ScalingScale );
            Display.Add( RingFan );
            DisplayTabs.Add( Width );
        }
    }
}
