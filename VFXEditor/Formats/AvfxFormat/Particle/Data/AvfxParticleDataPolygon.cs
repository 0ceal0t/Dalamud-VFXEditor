using VFXEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataPolygon : AvfxData {
        public readonly AvfxCurve1Axis Count = new( "Count", "Cnt" );
        public readonly AvfxCurve1Axis CountRandom = new( "Count Random", "CntR" );

        public AvfxParticleDataPolygon() : base() {
            Parsed = [
                Count,
                CountRandom
            ];

            Tabs.Add( Count );
            Tabs.Add( CountRandom );
        }
    }
}
