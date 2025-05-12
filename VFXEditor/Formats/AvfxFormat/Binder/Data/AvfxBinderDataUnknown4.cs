using VFXEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.AvfxFormat {
    public class AvfxBinderDataUnknown4 : AvfxData {
        public readonly AvfxCurve1Axis CarryOverFactor = new( "Carry Over Factor", "COF" );

        public AvfxBinderDataUnknown4() : base() {
            Parsed = [
                CarryOverFactor
            ];

            Tabs.Add( CarryOverFactor );
        }
    }
}
