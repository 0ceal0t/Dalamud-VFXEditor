namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataLaser : AvfxData {
        public readonly AvfxCurve Length = new( "Length", "Len" );
        public readonly AvfxCurve Width = new( "Width", "Wdt" );

        public AvfxParticleDataLaser() : base() {
            Parsed = new() {
                Length,
                Width
            };

            DisplayTabs.Add( Width );
            DisplayTabs.Add( Length );
        }
    }
}
