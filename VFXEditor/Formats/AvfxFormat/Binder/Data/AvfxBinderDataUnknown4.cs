namespace VfxEditor.AvfxFormat {
    public class AvfxBinderDataUnknown4 : AvfxData {
        public readonly AvfxCurve CarryOverFactor = new( "Carry Over Factor", "COF" );

        public AvfxBinderDataUnknown4() : base() {
            Parsed = [
                CarryOverFactor
            ];

            DisplayTabs.Add( CarryOverFactor );
        }
    }
}
