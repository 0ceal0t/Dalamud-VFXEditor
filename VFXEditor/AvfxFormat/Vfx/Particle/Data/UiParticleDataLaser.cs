using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticleDataLaser : UiData {
        public UiParticleDataLaser( AVFXParticleDataLaser data ) {
            Tabs.Add( new UiCurve( data.Width, "Width" ) );
            Tabs.Add( new UiCurve( data.Length, "Length" ) );
        }
    }
}
