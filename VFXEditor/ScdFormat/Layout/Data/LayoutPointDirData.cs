using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class LayoutPointDirData : ScdLayoutData {
        public readonly ParsedFloat4 Position = new( "Position" );
        public readonly ParsedFloat4 Direction = new( "Direction" );
        public readonly ParsedFloat RangeX = new( "Range X" );
        public readonly ParsedFloat RangeY = new( "Range Y" );
        public readonly ParsedFloat MaxRange = new( "Max Range" );
        public readonly ParsedFloat MinRange = new( "Min Range" );
        public readonly ParsedFloat2 Height = new( "Height" );
        public readonly ParsedFloat RangeVolume = new( "Range Volume" );
        public readonly ParsedFloat Volume = new( "Volume" );
        public readonly ParsedFloat Pitch = new( "Pitch" );
        public readonly ParsedFloat ReverbFac = new( "Reverb FAC" );
        public readonly ParsedFloat DopplerFac = new( "Doppler FAC" );
        public readonly ParsedFloat InteriorFac = new( "Interior FAC" );
        public readonly ParsedFloat FixedDirection = new( "Fixed Direction" );
        public readonly ParsedReserve Reserved1 = new( 3 * 4 );

        public LayoutPointDirData() {
            Parsed = new() {
                Position,
                Direction,
                RangeX,
                RangeY,
                MaxRange,
                MinRange,
                Height,
                RangeVolume,
                Volume,
                Pitch,
                ReverbFac,
                DopplerFac,
                InteriorFac,
                FixedDirection,
                Reserved1
            };
        }
    }
}
