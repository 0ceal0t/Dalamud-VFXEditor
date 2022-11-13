using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleDataDecal : AvfxData {
        public readonly AvfxFloat ScalingScale = new( "Scaling Scale", "SS" );

        public readonly UiParameters Parameters;

        public AvfxParticleDataDecal() : base() {
            Children = new() {
                ScalingScale
            };

            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( ScalingScale );
        }
    }
}
