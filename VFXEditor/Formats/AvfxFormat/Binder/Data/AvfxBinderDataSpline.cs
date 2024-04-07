namespace VfxEditor.AvfxFormat {
    public class AvfxBinderDataSpline : AvfxData {
        public readonly AvfxCurve CarryOverFactor = new( "Carry Over Factor", "COF" );
        public readonly AvfxCurve CarryOverFactorRandom = new( "Carry Over Factor Random", "COFR" );

        public AvfxBinderDataSpline() : base() {
            Parsed = [
                CarryOverFactor,
                CarryOverFactorRandom
            ];

            Tabs.Add( CarryOverFactor );
            Tabs.Add( CarryOverFactorRandom );
        }
    }
}
