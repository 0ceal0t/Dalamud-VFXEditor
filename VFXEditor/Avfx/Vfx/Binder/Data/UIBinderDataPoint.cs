using VFXEditor.AVFXLib.Binder;

namespace VFXEditor.Avfx.Vfx {
    public class UIBinderDataPoint : UIData {
        public UIBinderDataPoint( AVFXBinderDataPoint data ) {
            Tabs.Add( new UICurve( data.SpringStrength, "Spring Strength" ) );
        }
    }
}
