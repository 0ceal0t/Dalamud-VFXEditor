using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AVFX.VFX {
    public class UIParticleDataWindmill : UIData {
        public UIParameters Parameters;

        public UIParticleDataWindmill( AVFXParticleDataWindmill data ) {
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UICombo<WindmillUVType>( "Windmill UV Type", data.WindmillUVType ) );
        }
    }
}
