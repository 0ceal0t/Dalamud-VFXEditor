using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    [Flags]
    public enum PolygonFlags {
        IsReverbObject = 0x80
    }

    public class LayoutPolygonData : ScdLayoutData {
        public readonly ParsedFloat MaxRange = new( "Max Range" );
        public readonly ParsedFloat MinRange = new( "Min Range" );
        public readonly ParsedFloat2 Height = new( "Height" );
        public readonly ParsedFloat RangeVolume = new( "Range Volume" );
        public readonly ParsedFloat Volume = new( "Volume" );
        public readonly ParsedFloat Pitch = new( "Pitch" );
        public readonly ParsedFloat ReverbFac = new( "Reverb FAC" );
        public readonly ParsedFloat DopplerFac = new( "Doppler FAC" );
        public readonly ParsedFloat InteriorFac = new( "Interior FAC" );
        public readonly ParsedFloat Direction = new( "Direction" );
        public readonly ParsedByte SubSoundType = new( "Sub-Sound Type" );
        public readonly ParsedFlag<PolygonFlags> Flag = new( "Flag", size: 1 );
        public readonly ParsedByte VertexCount = new( "Vertex Count" );
        public readonly ParsedReserve Reserved1 = new( 1 );
        public readonly ParsedFloat RotSpeed = new( "Rotation Speed" );
        public readonly ParsedReserve Reserved2 = new( 3 * 4 );

        public LayoutPolygonData() {
            Parsed = new() {
                MaxRange,
                MinRange,
                Height,
                RangeVolume,
                Volume,
                Pitch,
                ReverbFac,
                DopplerFac,
                InteriorFac,
                Direction,
                SubSoundType,
                Flag,
                VertexCount,
                Reserved1,
                RotSpeed,
                Reserved2,
                // Positions go here
            };

            for( var i = 0; i < 32; i++ ) Parsed.Add( new ParsedFloat4( $"Position {i}" ) );
        }
    }
}
