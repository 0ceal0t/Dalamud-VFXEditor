using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataPowder : AvfxDataWithParameters {
        public readonly AvfxBool IsLightning = new( "Is Lightning", "bLgt" );
        public readonly AvfxEnum<DirectionalLightType> DirectionalLightType = new( "Directional Light Type", "LgtT" );
        public readonly AvfxFloat CenterOffset = new( "Center Offset", "CnOf" );

        public AvfxParticleDataPowder() : base() {
            Parsed = [
                IsLightning,
                DirectionalLightType,
                CenterOffset
            ];

            ParameterTab.Add( DirectionalLightType );
            ParameterTab.Add( IsLightning );
            ParameterTab.Add( CenterOffset );
        }
    }
}
