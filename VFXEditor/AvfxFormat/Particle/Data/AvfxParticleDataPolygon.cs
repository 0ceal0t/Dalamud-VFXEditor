namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataPolygon : AvfxData {
        public readonly AvfxCurve Count = new( "Count", "Cnt" );

        public AvfxParticleDataPolygon() : base() {
            Parsed = new() {
                Count
            };

            DisplayTabs.Add( Count );
        }
    }
}
