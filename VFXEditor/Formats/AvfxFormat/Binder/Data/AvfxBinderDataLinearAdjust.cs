using VfxEditor.Formats.AvfxFormat.Base;
using VfxEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.Formats.AvfxFormat.Binder.Data {
    public class AvfxBinderDataLinearAdjust : AvfxData {
        public readonly AvfxCurve1Axis CarryOverFactor = new( "Carry Over Factor", "COF" );

        public AvfxBinderDataLinearAdjust() : base() {
            Parsed = GetFieldsInOrder<AvfxBase>( typeof( AvfxBinderDataLinearAdjust ) );

            Tabs.Add( CarryOverFactor );
        }
    }
}
