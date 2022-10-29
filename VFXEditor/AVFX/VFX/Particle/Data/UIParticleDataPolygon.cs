using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AVFX.VFX {
    public class UIParticleDataPolygon : UIData {
        public UIParticleDataPolygon( AVFXParticleDataPolygon data ) {
            Tabs.Add( new UICurve( data.Count, "Count" ) );
        }
    }
}
