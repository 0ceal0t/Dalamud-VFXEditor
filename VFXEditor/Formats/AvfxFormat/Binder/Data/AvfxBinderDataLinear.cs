using VfxEditor.Formats.AvfxFormat.Base;
using VfxEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.Formats.AvfxFormat.Binder.Data {
    public class AvfxBinderDataLinear : AvfxData {
        public readonly AvfxCurve1Axis CarryOverFactor = new( "Carry Over Factor", "COF" );
        public readonly AvfxCurve1Axis CarryOverFactorRandom = new( "Carry Over Factor Random", "COFR" );

        public AvfxBinderDataLinear() : base() {
            Parsed = GetFieldsInOrder<AvfxBase>( typeof( AvfxBinderDataLinear ) );

            Tabs.Add( CarryOverFactor );
            Tabs.Add( CarryOverFactorRandom );
        }
    }
}
