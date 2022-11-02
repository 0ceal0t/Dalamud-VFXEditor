using VfxEditor.AVFXLib.Binder;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiBinderDataPoint : UiData {
        public UiBinderDataPoint( AVFXBinderDataPoint data ) {
            Tabs.Add( new UiCurve( data.SpringStrength, "Spring Strength" ) );
        }
    }
}
