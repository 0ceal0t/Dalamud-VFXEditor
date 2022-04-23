using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Particle;

namespace VFXEditor.AVFX.VFX {
    public class UIParticleDataPowder : UIData {
        public UIParameters Parameters;

        public UIParticleDataPowder( AVFXParticleDataPowder data ) {
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UICombo<DirectionalLightType>( "Directional Light Type", data.DirectionalLightType ) );
            Parameters.Add( new UICheckbox( "Is Lightning", data.IsLightning ) );
            Parameters.Add( new UIFloat( "Center Offset", data.CenterOffset ) );
        }
    }
}
