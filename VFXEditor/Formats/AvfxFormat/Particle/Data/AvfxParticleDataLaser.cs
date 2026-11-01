using VfxEditor.Formats.AvfxFormat.Base;
using VfxEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.Formats.AvfxFormat.Particle.Data {
    public class AvfxParticleDataLaser : AvfxData {
        public readonly AvfxCurve1Axis Length = new( "Length", "Len" );
        public readonly AvfxCurve1Axis LengthRandom = new( "Length Random", "LenR" );
        public readonly AvfxCurve1Axis Width = new( "Width", "Wdt" );
        public readonly AvfxCurve1Axis WidthRandom = new( "Width Random", "WdtR" );

        public AvfxParticleDataLaser() : base() {
            Parsed = GetFieldsInOrder<AvfxBase>();

            Tabs.Add( Length );
            Tabs.Add( LengthRandom );
            Tabs.Add( Width );
            Tabs.Add( WidthRandom );
        }
    }
}
