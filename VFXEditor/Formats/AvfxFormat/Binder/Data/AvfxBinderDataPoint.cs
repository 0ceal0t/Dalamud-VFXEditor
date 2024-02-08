namespace VfxEditor.AvfxFormat {
    public class AvfxBinderDataPoint : AvfxData {
        public readonly AvfxCurve SpringStrength = new( "Spring Strength", "SpS" );

        public AvfxBinderDataPoint() : base() {
            Parsed = [
                SpringStrength
            ];

            DisplayTabs.Add( SpringStrength );
        }
    }
}
