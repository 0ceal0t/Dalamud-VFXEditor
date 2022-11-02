using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticleDataDecalRing : UiData {
        public UiParameters Parameters;

        public UiParticleDataDecalRing( AVFXParticleDataDecalRing data ) {
            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( new UiFloat( "Scaling Scale", data.ScalingScale ) );
            Parameters.Add( new UiFloat( "Ring Fan", data.RingFan ) );
            Tabs.Add( new UiCurve( data.Width, "Width" ) );
        }
    }
}
