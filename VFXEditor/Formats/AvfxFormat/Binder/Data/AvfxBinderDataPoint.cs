using VFXEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.AvfxFormat {
    public class AvfxBinderDataPoint : AvfxData {
        public readonly AvfxCurve1Axis SpringStrength = new( "Spring Strength", "SpS" );

        public AvfxBinderDataPoint() : base() {
            Parsed = [
                SpringStrength
            ];

            Tabs.Add( SpringStrength );
        }
    }
}
