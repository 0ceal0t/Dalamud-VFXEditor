using VfxEditor.Formats.AvfxFormat.Base;
using VfxEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.Formats.AvfxFormat.Binder.Data {
    public class AvfxBinderDataPoint : AvfxData {
        public readonly AvfxCurve1Axis SpringStrength = new( "Spring Strength", "SpS" );
        public readonly AvfxCurve1Axis SpringStrengthRandom = new( "Spring Strength Random", "SpSR" );

        public AvfxBinderDataPoint() : base() {
            Parsed = GetFieldsInOrder<AvfxBase>();

            Tabs.Add( SpringStrength );
            Tabs.Add( SpringStrengthRandom );
        }
    }
}
