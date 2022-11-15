using System;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataPowder : AvfxData {
        public readonly AvfxBool IsLightning = new( "Is Lightning", "bLgt" );
        public readonly AvfxEnum<DirectionalLightType> DirectionalLightType = new( "Directional Light Type", "LgtT" );
        public readonly AvfxFloat CenterOffset = new( "Center Offset", "CnOf" );

        public readonly UiDisplayList Display;

        public AvfxParticleDataPowder() : base() {
            Parsed = new() {
                IsLightning,
                DirectionalLightType,
                CenterOffset
            };

            DisplayTabs.Add( Display = new UiDisplayList( "Parameters" ) );
            Display.Add( DirectionalLightType );
            Display.Add( IsLightning );
            Display.Add( CenterOffset );
        }
    }
}
