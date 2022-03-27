using AVFXLib.Models;

namespace VFXEditor.Avfx.Vfx {
    public class UIParticleDataPolygon : UIData {
        public UIParticleDataPolygon( AVFXParticleDataPolygon data ) {
            Tabs.Add( new UICurve( data.Count, "Count" ) );
        }
    }
}
