using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    [Flags]
    public enum ObstructionFlags {
        UseHiCutFac = 0x08
    }

    public class LayoutBoardObstructionData : ScdLayoutData {
        public readonly ParsedFloat4 Position1 = new( "Position 1" );
        public readonly ParsedFloat4 Position2 = new( "Position 2" );
        public readonly ParsedFloat4 Position3 = new( "Position 3" );
        public readonly ParsedFloat4 Position4 = new( "Position 4" );
        public readonly ParsedFloat ObstacleFac = new( "Obstacle FAC" );
        public readonly ParsedFloat HiCutFac = new( "Hi-Cut FAC" );
        public readonly ParsedFlag<ObstructionFlags> Flags = new( "Flags", size: 1 );
        public readonly ParsedReserve Reserved1 = new( 3 );
        public readonly ParsedShort OpenTime = new( "Open Time" );
        public readonly ParsedShort CloseTime = new( "Close Time" );

        public LayoutBoardObstructionData() {
            Parsed = new() {
                Position1,
                Position2,
                Position3,
                Position4,
                ObstacleFac,
                HiCutFac,
                Flags,
                Reserved1,
                OpenTime,
                CloseTime
            };
        }
    }
}
