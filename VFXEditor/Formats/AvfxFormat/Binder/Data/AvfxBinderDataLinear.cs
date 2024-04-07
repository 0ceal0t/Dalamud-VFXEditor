namespace VfxEditor.AvfxFormat {
    public class AvfxBinderDataLinear : AvfxData {
        public readonly AvfxCurve CarryOverFactor = new( "Carry Over Factor", "COF" );
        public readonly AvfxCurve CarryOverFactorRandom = new( "Carry Over Factor Random", "COFR" );

        public AvfxBinderDataLinear() : base() {
            Parsed = [
                CarryOverFactor,
                CarryOverFactorRandom
            ];

            Tabs.Add( CarryOverFactor );
            Tabs.Add( CarryOverFactorRandom );
        }
    }
}
