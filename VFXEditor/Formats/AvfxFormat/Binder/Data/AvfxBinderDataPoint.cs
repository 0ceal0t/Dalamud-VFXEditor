using VFXEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.AvfxFormat {
    public class AvfxBinderDataPoint : AvfxData {
        public readonly AvfxCurve1Axis SpringStrength = new( "Spring Strength", "SpS" );
        public readonly AvfxCurve1Axis SpringStrengthRandom = new( "Spring Strength Random", "SpSR" );

        public AvfxBinderDataPoint() : base() {
            Parsed = [
                SpringStrength,
                SpringStrengthRandom,
            ];

            Tabs.Add( SpringStrength );
            Tabs.Add( SpringStrengthRandom );
        }
    }
}
