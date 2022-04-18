using VFXEditor.AVFXLib.Particle;

namespace VFXEditor.Avfx.Vfx {
    public class UIParticleDataPolygon : UIData {
        public UIParticleDataPolygon( AVFXParticleDataPolygon data ) {
            Tabs.Add( new UICurve( data.Count, "Count" ) );
        }
    }
}
