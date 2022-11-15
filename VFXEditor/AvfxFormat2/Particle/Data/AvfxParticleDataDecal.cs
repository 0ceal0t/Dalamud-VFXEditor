using System;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleDataDecal : AvfxData {
        public readonly AvfxFloat ScalingScale = new( "Scaling Scale", "SS" );

        public readonly UiDisplayList Display;

        public AvfxParticleDataDecal() : base() {
            Parsed = new() {
                ScalingScale
            };

            DisplayTabs.Add( Display = new UiDisplayList( "Parameters" ) );
            Display.Add( ScalingScale );
        }
    }
}
