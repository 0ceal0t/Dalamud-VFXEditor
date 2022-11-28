using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class LayoutLineExtControllerData : ScdLayoutData {
        public readonly ParsedFloat4 StartPosition = new( "Start Position" );
        public readonly ParsedFloat4 EndPosition = new( "End Position" );
        public readonly ParsedFloat MaxRange = new( "Max Range" );
        public readonly ParsedFloat MinRange = new( "Min Range" );
        public readonly ParsedFloat2 Height = new( "Height" );
        public readonly ParsedFloat RangeVolume = new( "Range Volume" );
        public readonly ParsedFloat Volume = new( "Volume" );
        public readonly ParsedFloat LowerLimit = new( "Lower Limit" );
        public readonly ParsedInt FunctionNumber = new( "Function Number" );
        public readonly ParsedByte CalcType = new( "Calculation Type" );
        public readonly ParsedReserve Reserved1 = new( 3 + 4 * 4 );

        public LayoutLineExtControllerData() {
            Parsed = new() {
                StartPosition,
                EndPosition,
                MaxRange,
                MinRange,
                Height,
                RangeVolume,
                Volume,
                LowerLimit,
                FunctionNumber,
                CalcType,
                Reserved1
            };
        }
    }
}
