using VfxEditor.AVFXLib.Binder;

namespace VfxEditor.AVFX.VFX {
    public class UIBinderDataPoint : UIData {
        public UIBinderDataPoint( AVFXBinderDataPoint data ) {
            Tabs.Add( new UICurve( data.SpringStrength, "Spring Strength" ) );
        }
    }
}
