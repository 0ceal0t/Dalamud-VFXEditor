using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticleDataPowder : UiData {
        public UiParameters Parameters;

        public UiParticleDataPowder( AVFXParticleDataPowder data ) {
            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( new UiCombo<DirectionalLightType>( "Directional Light Type", data.DirectionalLightType ) );
            Parameters.Add( new UiCheckbox( "Is Lightning", data.IsLightning ) );
            Parameters.Add( new UiFloat( "Center Offset", data.CenterOffset ) );
        }
    }
}
