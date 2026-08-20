using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataPowder : AvfxDataWithParameters {
        public readonly AvfxBool MV = new( "Use Character Movement", "bMV" );
        public readonly AvfxBool Loc = new( "Use Character Location", "bLoc" );
        public readonly AvfxBool IsLightning = new( "Is Lightning", "bLgt" );
        public readonly AvfxEnum<DirectionalLightType> DirectionalLightType = new( "Directional Light Type", "LgtT" );
        public readonly AvfxFloat CenterOffset = new( "Center Offset", "CnOf" );

        public AvfxParticleDataPowder() : base() {
            Parsed = [
                MV,
                Loc,
                IsLightning,
                DirectionalLightType,
                CenterOffset
            ];

            ParameterTab.Add( MV );
            ParameterTab.Add( Loc );
            ParameterTab.Add( DirectionalLightType );
            ParameterTab.Add( IsLightning );
            ParameterTab.Add( CenterOffset );
        }
    }
}
