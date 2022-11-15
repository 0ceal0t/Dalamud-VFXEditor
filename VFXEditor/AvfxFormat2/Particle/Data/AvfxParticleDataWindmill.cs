using System;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleDataWindmill : AvfxData {
        public readonly AvfxEnum<WindmillUVType> WindmillUVType = new( "Windmill UV Type", "WUvT" );

        public readonly UiDisplayList Display;

        public AvfxParticleDataWindmill() : base() {
            Parsed = new() {
                WindmillUVType
            };

            DisplayTabs.Add( Display = new UiDisplayList( "Parameters" ) );
            Display.Add( WindmillUVType );
        }
    }
}
