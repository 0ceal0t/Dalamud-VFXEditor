using VFXEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataDecalRing : AvfxDataWithParameters {
        public readonly AvfxCurve1Axis Width = new( "Width", "WID" );
        public readonly AvfxCurve1Axis WidthRandom = new( "Width Random", "WIDR" );
        public readonly AvfxFloat ScalingScale = new( "Scaling Scale", "SS" );
        public readonly AvfxFloat RingFan = new( "Ring Fan", "RF" );
        public readonly AvfxInt DDTT = new( "DDTT", "DDTT" );

        public AvfxParticleDataDecalRing() : base() {
            Parsed = [
                Width,
                WidthRandom,
                ScalingScale,
                RingFan,
                DDTT
            ];

            ParameterTab.Add( ScalingScale );
            ParameterTab.Add( RingFan );
            ParameterTab.Add( DDTT );

            Tabs.Add( Width );
            Tabs.Add( WidthRandom );
        }
    }
}
