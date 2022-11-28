using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    [Flags]
    public enum SurfaceFlags {
        IsReverbObject = 0x80
    }

    public class LayoutSurfaceData : ScdLayoutData {
        public readonly ParsedFloat4 Position1 = new( "Position 1" );
        public readonly ParsedFloat4 Position2 = new( "Position 2" );
        public readonly ParsedFloat4 Position3 = new( "Position 3" );
        public readonly ParsedFloat4 Position4 = new( "Position 4" );
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
        public readonly ParsedFlag<SurfaceFlags> Flags = new( "Flags", size: 1 );
        public readonly ParsedReserve Reserved1 = new( 2 );
        public readonly ParsedFloat RotSpeed = new( "Rotation Speed" );
        public readonly ParsedReserve Reserved2 = new( 3 * 4 );

        public LayoutSurfaceData() {
            Parsed = new() {
                Position1,
                Position2,
                Position3,
                Position4,
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
                Flags,
                Reserved1,
                RotSpeed,
                Reserved2
            };
        }
    }
}
