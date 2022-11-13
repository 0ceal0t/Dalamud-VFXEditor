using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleDataPowder : AvfxData {
        public readonly AvfxBool IsLightning = new( "Is Lightning", "bLgt" );
        public readonly AvfxEnum<DirectionalLightType> DirectionalLightType = new( "Directional Light Type", "LgtT" );
        public readonly AvfxFloat CenterOffset = new( "Center Offset", "CnOf" );

        public readonly UiParameters Parameters;

        public AvfxParticleDataPowder() : base() {
            Children = new() {
                IsLightning,
                DirectionalLightType,
                CenterOffset
            };

            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( DirectionalLightType );
            Parameters.Add( IsLightning );
            Parameters.Add( CenterOffset );
        }
    }
}
