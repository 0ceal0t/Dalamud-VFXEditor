using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class LayoutPolylineData : ScdLayoutData {
        public readonly ParsedFloat MaxRange = new( "Max Range" );
        public readonly ParsedFloat MinRange = new( "Min Range" );
        public readonly ParsedFloat2 Height = new( "Height" );
        public readonly ParsedFloat RangeVolume = new( "Range Volume" );
        public readonly ParsedFloat Volume = new( "Volume" );
        public readonly ParsedFloat Pitch = new( "Pitch" );
        public readonly ParsedFloat ReverbFac = new( "Reverb FAC" );
        public readonly ParsedFloat DopplerFac = new( "Doppler FAC" );
        public readonly ParsedByte VertexCount = new( "Vertex Count" );
        public readonly ParsedReserve Reserved1 = new( 3 );
        public readonly ParsedFloat InteriorFac = new( "Interior FAC" );
        public readonly ParsedFloat Direction = new( "Direction" );

        public LayoutPolylineData() {
            Parsed = new() {
                // Positions go here
                MaxRange,
                MinRange,
                Height,
                RangeVolume,
                Volume,
                Pitch,
                ReverbFac,
                DopplerFac,
                VertexCount,
                Reserved1,
                InteriorFac,
                Direction
            };

            for( var i = 0; i < 16; i++ ) Parsed.Insert( 0, new ParsedFloat4( $"Position {15 - i}" ) );
        }
    }
}
