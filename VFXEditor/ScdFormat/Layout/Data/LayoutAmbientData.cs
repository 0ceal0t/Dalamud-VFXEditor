using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class LayoutAmbientData : ScdLayoutData {
        public readonly ParsedFloat Volume = new( "Volume" );
        public readonly ParsedFloat Pitch = new( "Pitch" );
        public readonly ParsedFloat ReverbFac = new( "Reverb FAC" );
        public readonly ParsedFloat4 DirectVolume1 = new( "Direct Volume 1" );
        public readonly ParsedFloat4 DirectVolume2 = new( "Direct Volume 2" );
        public readonly ParsedReserve Reserved = new( 4 );

        public LayoutAmbientData() {
            Parsed = new() {
                Volume,
                Pitch,
                ReverbFac,
                DirectVolume1,
                DirectVolume2,
                Reserved
            };
        }
    }
}
