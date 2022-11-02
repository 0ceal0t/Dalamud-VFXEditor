using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticleDataDecal : UiData {
        public UiParameters Parameters;

        public UiParticleDataDecal( AVFXParticleDataDecal data ) {
            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( new UiFloat( "Scaling Scale", data.ScalingScale ) );
        }
    }
}
