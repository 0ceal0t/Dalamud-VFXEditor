using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    [Flags]
    public enum PointEnvironmentFlags {
        IsUseEnvFilterDepth = 0x40,
        IsFireWorks = 0x80
    }

    [Flags]
    public enum PointFlags {
        IsReverbObject = 0x40,
        IsWhizGenerate = 0x80
    }

    public class LayoutPointData : ScdLayoutData {
        public readonly ParsedFloat4 Position = new( "Position" );
        public readonly ParsedFloat MaxRange = new( "Max Range" );
        public readonly ParsedFloat MinRange = new( "Min Range" );
        public readonly ParsedFloat2 Height = new( "Height" );
        public readonly ParsedFloat RangeVolume = new( "Range Volume" );
        public readonly ParsedFloat Volume = new( "Volume" );
        public readonly ParsedFloat Pitch = new( "Pitch" );
        public readonly ParsedFloat ReverbFac = new( "Reverb FAC" );
        public readonly ParsedFloat DopplerFac = new( "Doppler FAC" );
        public readonly ParsedFloat CenterFac = new( "Center FAC" );
        public readonly ParsedFloat InteriorFac = new( "Interior FAC" );
        public readonly ParsedFloat Direction = new( "Direction" );
        public readonly ParsedFloat NearFadeStart = new( "Near Fade Start" );
        public readonly ParsedFloat NearFadeEnd = new( "Near Fade End" );
        public readonly ParsedFloat FarDelayFac = new( "Far Delay FAC" );
        public readonly ParsedFlag<PointEnvironmentFlags> Environment = new( "Environment", size: 1 );
        public readonly ParsedFlag<PointFlags> Flag = new( "Flag", size: 1 );
        public readonly ParsedReserve Reserved1 = new( 2 );
        public readonly ParsedFloat LowerLimit = new( "Lower Limit" );
        public readonly ParsedShort FadeInTime = new( "Fade In Time" );
        public readonly ParsedShort FadeOutTime = new( "Fade Out Time" );
        public readonly ParsedFloat ConvergenceFac = new( "Convergence FAC" );
        public readonly ParsedReserve Reserved2 = new( 4 );

        public LayoutPointData() {
            Parsed = new() {
                Position,
                MaxRange,
                MinRange,
                Height,
                RangeVolume,
                Volume,
                Pitch,
                ReverbFac,
                DopplerFac,
                CenterFac,
                InteriorFac,
                Direction,
                NearFadeStart,
                NearFadeEnd,
                FarDelayFac,
                Environment,
                Flag,
                Reserved1,
                LowerLimit,
                FadeInTime,
                FadeOutTime,
                ConvergenceFac,
                Reserved2
            };
        }
    }
}
