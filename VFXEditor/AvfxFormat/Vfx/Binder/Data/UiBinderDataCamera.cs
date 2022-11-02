using VfxEditor.AVFXLib.Binder;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiBinderDataCamera : UiData {
        public UiBinderDataCamera( AVFXBinderDataCamera data ) {
            Tabs.Add( new UiCurve( data.Distance, "Distance" ) );
            Tabs.Add( new UiCurve( data.DistanceRandom, "Distance Random" ) );
        }
    }
}
