using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleDataDecal : AvfxData {
        public readonly AvfxFloat ScalingScale = new( "Scaling Scale", "SS" );

        public readonly UiParameters Display;

        public AvfxParticleDataDecal() : base() {
            Parsed = new() {
                ScalingScale
            };

            DisplayTabs.Add( Display = new UiParameters( "Parameters" ) );
            Display.Add( ScalingScale );
        }
    }
}
