using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class LayoutPolylineObstructionData : ScdLayoutData {
        public readonly ParsedFloat2 Height = new( "Height" );
        public readonly ParsedFloat ObstacleFac = new( "Obstacle FAC" );
        public readonly ParsedFloat HiCutFac = new( "Hi-Cut FAC" );
        public readonly ParsedFlag<ObstructionFlags> Flags = new( "Flags", size: 1 );
        public readonly ParsedByte VertexCount = new( "Vertex Count" );
        public readonly ParsedReserve Reserved1 = new( 2 );
        public readonly ParsedFloat Width = new( "Width" );
        public readonly ParsedFloat FadeRange = new( "Fade Range" );
        public readonly ParsedShort OpenTime = new( "Open Time" );
        public readonly ParsedShort CloseTime = new( "Close Time" );

        public LayoutPolylineObstructionData() {
            Parsed = new() {
                // Positions go here
                Height,
                ObstacleFac,
                HiCutFac,
                Flags,
                VertexCount,
                Reserved1,
                Width,
                FadeRange,
                OpenTime,
                CloseTime
            };

            for( var i = 0; i < 16; i++ ) Parsed.Insert( 0, new ParsedFloat4( $"Position {15 - i}" ) );
        }
    }
}
