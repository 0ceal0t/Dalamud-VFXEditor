using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class LayoutDirectionData : ScdLayoutData {
        public readonly ParsedFloat Volume = new( "Volume" );
        public readonly ParsedFloat Pitch = new( "Pitch" );
        public readonly ParsedFloat ReverbFac = new( "Reverb FAC" );
        public readonly ParsedFloat Direction = new( "Direction" );
        public readonly ParsedFloat RotSpeed = new( "Rotation Speed" );
        public readonly ParsedReserve Reserved = new( 3 * 4 );

        public LayoutDirectionData() {
            Parsed = new() {
                Volume,
                Pitch,
                ReverbFac,
                Direction,
                RotSpeed,
                Reserved
            };
        }
    }
}
