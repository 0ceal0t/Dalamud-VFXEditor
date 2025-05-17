using VFXEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.AvfxFormat {
    public class AvfxBinderDataLinearAdjust : AvfxData {
        public readonly AvfxCurve1Axis CarryOverFactor = new( "Carry Over Factor", "COF" );

        public AvfxBinderDataLinearAdjust() : base() {
            Parsed = [
                CarryOverFactor
            ];

            Tabs.Add( CarryOverFactor );
        }
    }
}
