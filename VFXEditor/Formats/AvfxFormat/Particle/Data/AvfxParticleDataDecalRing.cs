using VfxEditor.Formats.AvfxFormat.Base;
using VfxEditor.Formats.AvfxFormat.Components.Float;
using VfxEditor.Formats.AvfxFormat.Components.Int;
using VfxEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.Formats.AvfxFormat.Particle.Data {
    public class AvfxParticleDataDecalRing : AvfxDataWithParameters {
        public readonly AvfxCurve1Axis Width = new( "Width", "WID" );
        public readonly AvfxCurve1Axis WidthRandom = new( "Width Random", "WIDR" );
        public readonly AvfxFloat ScalingScale = new( "Scaling Scale", "SS" );
        public readonly AvfxFloat RingFan = new( "Ring Fan", "RF" );
        public readonly AvfxInt DDTT = new( "DDTT", "DDTT" );

        public AvfxParticleDataDecalRing() : base() {
            Parsed = GetFieldsInOrder<AvfxBase>( typeof( AvfxParticleDataDecalRing ) );

            ParameterTab.Add( ScalingScale );
            ParameterTab.Add( RingFan );
            ParameterTab.Add( DDTT );

            Tabs.Add( Width );
            Tabs.Add( WidthRandom );
        }
    }
}
