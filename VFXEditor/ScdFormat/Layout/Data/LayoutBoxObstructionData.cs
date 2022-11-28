using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class LayoutBoxObstructionData : ScdLayoutData {
        public readonly ParsedFloat4 Position1 = new( "Position 1" );
        public readonly ParsedFloat4 Position2 = new( "Position 2" );
        public readonly ParsedFloat4 Position3 = new( "Position 3" );
        public readonly ParsedFloat4 Position4 = new( "Position 4" );
        public readonly ParsedFloat2 Height = new( "Height" );
        public readonly ParsedFloat ObstacleFac = new( "Obstacle FAC" );
        public readonly ParsedFloat HiCutFac = new( "Hi-Cut FAC" );
        public readonly ParsedFlag<ObstructionFlags> Flags = new( "Flags", size: 1 );
        public readonly ParsedReserve Reserved1 = new( 3 );
        public readonly ParsedFloat FadeRange = new( "Fade Range" );
        public readonly ParsedShort OpenTime = new( "Open Time" );
        public readonly ParsedShort CloseTime = new( "Close Time" );
        public readonly ParsedReserve Reserved2 = new( 4 );

        public LayoutBoxObstructionData() {
            Parsed = new() {
                Position1,
                Position2,
                Position3,
                Position4,
                Height,
                ObstacleFac,
                HiCutFac,
                Flags,
                Reserved1,
                FadeRange,
                OpenTime,
                CloseTime,
                Reserved2
            };
        }
    }
}
