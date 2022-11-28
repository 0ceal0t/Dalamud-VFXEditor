using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class LayoutPolygonObstructionData : ScdLayoutData {
        public readonly ParsedFloat ObstacleFac = new( "Obstacle FAC" );
        public readonly ParsedFloat HiCutFac = new( "Hi-Cut FAC" );
        public readonly ParsedFlag<ObstructionFlags> Flags = new( "Flags", size: 1 );
        public readonly ParsedByte VertexCount = new( "Vertex Count" );
        public readonly ParsedReserve Reserved1 = new( 2 );
        public readonly ParsedShort OpenTime = new( "Open Time" );
        public readonly ParsedShort CloseTime = new( "Close Time" );

        public LayoutPolygonObstructionData() {
            Parsed = new() {
                // Positions go here
                ObstacleFac,
                HiCutFac,
                Flags,
                VertexCount,
                Reserved1,
                OpenTime,
                CloseTime
            };

            for( var i = 0; i < 32; i++ ) Parsed.Insert( 0, new ParsedFloat4( $"Position {15 - i}" ) );
        }
    }
}
