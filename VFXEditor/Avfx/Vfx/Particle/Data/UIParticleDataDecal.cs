using AVFXLib.Models;

namespace VFXEditor.Avfx.Vfx {
    public class UIParticleDataDecal : UIData {
        public UIParameters Parameters;

        public UIParticleDataDecal( AVFXParticleDataDecal data ) {
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UIFloat( "Scaling Scale", data.ScalingScale ) );
        }
    }
}
