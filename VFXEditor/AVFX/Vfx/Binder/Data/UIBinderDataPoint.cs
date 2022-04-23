using VFXEditor.AVFXLib.Binder;

namespace VFXEditor.AVFX.VFX {
    public class UIBinderDataPoint : UIData {
        public UIBinderDataPoint( AVFXBinderDataPoint data ) {
            Tabs.Add( new UICurve( data.SpringStrength, "Spring Strength" ) );
        }
    }
}
