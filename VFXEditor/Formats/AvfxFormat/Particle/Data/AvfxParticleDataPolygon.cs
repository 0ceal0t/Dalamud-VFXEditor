using VfxEditor.Formats.AvfxFormat.Base;
using VfxEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.Formats.AvfxFormat.Particle.Data {
    public class AvfxParticleDataPolygon : AvfxData {
        public readonly AvfxCurve1Axis Count = new( "Count", "Cnt" );
        public readonly AvfxCurve1Axis CountRandom = new( "Count Random", "CntR" );

        public AvfxParticleDataPolygon() : base() {
            Parsed = GetFieldsInOrder<AvfxBase>( typeof( AvfxParticleDataPolygon ) );

            Tabs.Add( Count );
            Tabs.Add( CountRandom );
        }
    }
}
