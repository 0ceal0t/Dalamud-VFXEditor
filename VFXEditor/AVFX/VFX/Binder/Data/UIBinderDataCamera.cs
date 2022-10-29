using VfxEditor.AVFXLib.Binder;

namespace VfxEditor.AVFX.VFX {
    public class UIBinderDataCamera : UIData {
        public UIBinderDataCamera( AVFXBinderDataCamera data ) {
            Tabs.Add( new UICurve( data.Distance, "Distance" ) );
            Tabs.Add( new UICurve( data.DistanceRandom, "Distance Random" ) );
        }
    }
}
