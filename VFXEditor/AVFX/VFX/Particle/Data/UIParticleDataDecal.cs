using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AVFX.VFX {
    public class UIParticleDataDecal : UIData {
        public UIParameters Parameters;

        public UIParticleDataDecal( AVFXParticleDataDecal data ) {
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UIFloat( "Scaling Scale", data.ScalingScale ) );
        }
    }
}
