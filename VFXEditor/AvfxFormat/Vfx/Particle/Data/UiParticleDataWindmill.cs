using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticleDataWindmill : UiData {
        public UiParameters Parameters;

        public UiParticleDataWindmill( AVFXParticleDataWindmill data ) {
            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( new UiCombo<WindmillUVType>( "Windmill UV Type", data.WindmillUVType ) );
        }
    }
}
