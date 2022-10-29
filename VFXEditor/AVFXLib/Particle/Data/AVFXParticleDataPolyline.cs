using System.Collections.Generic;
using VfxEditor.AVFXLib.Curve;

namespace VfxEditor.AVFXLib.Particle {
    public class AVFXParticleDataPolyline : AVFXGenericData {
        public readonly AVFXEnum<LineCreateType> CreateLineType = new( "LnCT" );
        public readonly AVFXEnum<NotBillboardBaseAxisType> NotBillBoardBaseAxisType = new( "NBBA" );
        public readonly AVFXInt BindWeaponType = new( "BWpT" );
        public readonly AVFXInt PointCount = new( "PnC" );
        public readonly AVFXInt PointCountCenter = new( "PnCC" );
        public readonly AVFXInt PointCountEndDistortion = new( "PnED" );
        public readonly AVFXBool UseEdge = new( "bEdg" );
        public readonly AVFXBool NotBillboard = new( "bNtB" );
        public readonly AVFXBool BindWeapon = new( "BdWp" );
        public readonly AVFXBool ConnectTarget = new( "bCtg" );
        public readonly AVFXBool ConnectTargetReverse = new( "bCtr" );
        public readonly AVFXInt TagNumber = new( "TagN" );
        public readonly AVFXBool IsSpline = new( "bSpl" );
        public readonly AVFXBool IsLocal = new( "bLcl" );

        public readonly AVFXCurve CF = new( "CF" );
        public readonly AVFXCurve Width = new( "Wd" );
        public readonly AVFXCurve WidthRandom = new( "WdR" );
        public readonly AVFXCurve WidthBegin = new( "WdB" );
        public readonly AVFXCurve WidthCenter = new( "WdC" );
        public readonly AVFXCurve WidthEnd = new( "WdE" );
        public readonly AVFXCurve Length = new( "Len" );
        public readonly AVFXCurve LengthRandom = new( "LenR" );
        public readonly AVFXCurve Softness = new( "Sft" );
        public readonly AVFXCurve SoftRandom = new( "SftR" );
        public readonly AVFXCurve PnDs = new( "PnDs" );
        public readonly AVFXCurveColor ColorBegin = new( name: "ColB" );
        public readonly AVFXCurveColor ColorCenter = new( name: "ColC" );
        public readonly AVFXCurveColor ColorEnd = new( name: "ColE" );
        public readonly AVFXCurveColor ColorEdgeBegin = new( name: "CoEB" );
        public readonly AVFXCurveColor ColorEdgeCenter = new( name: "CoEC" );
        public readonly AVFXCurveColor ColorEdgeEnd = new( name: "CoEE" );

        public AVFXParticleDataPolyline() : base() {
            Children = new List<AVFXBase> {
                CreateLineType,
                NotBillBoardBaseAxisType,
                BindWeaponType,
                PointCount,
                PointCountCenter,
                PointCountEndDistortion,
                UseEdge,
                NotBillboard,
                BindWeapon,
                ConnectTarget,
                ConnectTargetReverse,
                TagNumber,
                IsSpline,
                IsLocal,
                CF,
                Width,
                WidthRandom,
                WidthBegin,
                WidthCenter,
                WidthEnd,
                Length,
                LengthRandom,
                Softness,
                SoftRandom,
                PnDs,
                ColorBegin,
                ColorCenter,
                ColorEnd,
                ColorEdgeBegin,
                ColorEdgeCenter,
                ColorEdgeEnd
            };
        }
    }
}
