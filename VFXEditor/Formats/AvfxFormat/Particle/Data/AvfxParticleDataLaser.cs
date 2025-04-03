using VFXEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataLaser : AvfxData {
        public readonly AvfxCurve1Axis Length = new( "Length", "Len" );
        public readonly AvfxCurve1Axis Width = new( "Width", "Wdt" );

        public AvfxParticleDataLaser() : base() {
            Parsed = [
                Length,
                Width
            ];

            Tabs.Add( Width );
            Tabs.Add( Length );
        }
    }
}
