using VFXEditor.AVFXLib.Particle;

namespace VFXEditor.AVFX.VFX {
    public class UIParticleDataPolygon : UIData {
        public UIParticleDataPolygon( AVFXParticleDataPolygon data ) {
            Tabs.Add( new UICurve( data.Count, "Count" ) );
        }
    }
}
