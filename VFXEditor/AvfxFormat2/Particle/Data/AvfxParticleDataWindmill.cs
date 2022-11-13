using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleDataWindmill : AvfxData {
        public readonly AvfxEnum<WindmillUVType> WindmillUVType = new( "Windmill UV Type", "WUvT" );

        public readonly UiParameters Display;

        public AvfxParticleDataWindmill() : base() {
            Parsed = new() {
                WindmillUVType
            };

            DisplayTabs.Add( Display = new UiParameters( "Parameters" ) );
            Display.Add( WindmillUVType );
        }
    }
}
