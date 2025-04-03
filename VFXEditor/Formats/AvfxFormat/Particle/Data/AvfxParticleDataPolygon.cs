using VFXEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataPolygon : AvfxData {
        public readonly AvfxCurve1Axis Count = new( "Count", "Cnt" );

        public AvfxParticleDataPolygon() : base() {
            Parsed = [
                Count
            ];

            Tabs.Add( Count );
        }
    }
}
