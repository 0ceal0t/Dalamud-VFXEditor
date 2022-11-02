using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticleDataPolygon : UiData {
        public UiParticleDataPolygon( AVFXParticleDataPolygon data ) {
            Tabs.Add( new UiCurve( data.Count, "Count" ) );
        }
    }
}
