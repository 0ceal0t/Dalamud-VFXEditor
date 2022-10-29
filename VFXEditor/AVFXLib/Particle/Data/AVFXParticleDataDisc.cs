using System.Collections.Generic;
using VfxEditor.AVFXLib.Curve;

namespace VfxEditor.AVFXLib.Particle {
    public class AVFXParticleDataDisc : AVFXGenericData {
        public readonly AVFXInt PartsCount = new( "PrtC" );
        public readonly AVFXInt PartsCountU = new( "PCnU" );
        public readonly AVFXInt PartsCountV = new( "PCnV" );
        public readonly AVFXFloat PointIntervalFactoryV = new( "PIFU" );
        public readonly AVFXCurve Angle = new( "Ang" );
        public readonly AVFXCurve HeightBeginInner = new( "HBI" );
        public readonly AVFXCurve HeightEndInner = new( "HEI" );
        public readonly AVFXCurve HeightBeginOuter = new( "HBO" );
        public readonly AVFXCurve HeightEndOuter = new( "HEO" );
        public readonly AVFXCurve WidthBegin = new( "WB" );
        public readonly AVFXCurve WidthEnd = new( "WE" );
        public readonly AVFXCurve RadiusBegin = new( "RB" );
        public readonly AVFXCurve RadiusEnd = new( "RE" );
        public readonly AVFXCurveColor ColorEdgeInner = new( name: "CEI" );
        public readonly AVFXCurveColor ColorEdgeOuter = new( name: "CEO" );

        public AVFXParticleDataDisc() : base() {
            Children = new List<AVFXBase> {
                PartsCount,
                PartsCountU,
                PartsCountV,
                PointIntervalFactoryV,
                Angle,
                HeightBeginInner,
                HeightEndInner,
                HeightBeginOuter,
                HeightEndOuter,
                WidthBegin,
                WidthEnd,
                RadiusBegin,
                RadiusEnd,
                ColorEdgeInner,
                ColorEdgeOuter
            };
        }
    }
}
