using VFXEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.AvfxFormat {
    public class AvfxBinderDataSpline : AvfxData {
        public readonly AvfxCurve1Axis CarryOverFactor = new( "Carry Over Factor", "COF" );
        public readonly AvfxCurve1Axis CarryOverFactorRandom = new( "Carry Over Factor Random", "COFR" );

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
