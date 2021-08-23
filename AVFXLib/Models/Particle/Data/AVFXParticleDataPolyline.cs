using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class AVFXParticleDataPolyline : AVFXParticleData {
        public LiteralEnum<LineCreateType> CreateLineType = new( "LnCT" );
        public LiteralEnum<NotBillboardBaseAxisType> NotBillBoardBaseAxisType = new( "NBBA" );
        public LiteralInt BindWeaponType = new( "BWpT" );
        public LiteralInt PointCount = new( "PnC" );
        public LiteralInt PointCountCenter = new( "PnCC" );
        public LiteralInt PointCountEndDistortion = new( "PnED" );
        public LiteralBool UseEdge = new( "bEdg" );
        public LiteralBool NotBillboard = new( "bNtB" );
        public LiteralBool BindWeapon = new( "BdWp" );
        public LiteralBool ConnectTarget = new( "bCtg" );
        public LiteralBool ConnectTargetReverse = new( "bCtr" );
        public LiteralInt TagNumber = new( "TagN" );
        public LiteralBool IsSpline = new( "bSpl" );
        public LiteralBool IsLocal = new( "bLcl" );

        public AVFXCurve CF = new( "CF" );
        public AVFXCurve Width = new( "Wd" );
        public AVFXCurve WidthRandom = new( "WdR" );
        public AVFXCurve WidthBegin = new( "WdB" );
        public AVFXCurve WidthCenter = new( "WdC" );
        public AVFXCurve WidthEnd = new( "WdE" );
        public AVFXCurve Length = new( "Len" );
        public AVFXCurve LengthRandom = new( "LenR" );
        public AVFXCurve Softness = new( "Sft" );
        public AVFXCurve SoftRandom = new( "SftR" );
        public AVFXCurve PnDs = new( "PnDs" );
        public AVFXCurveColor ColorBegin = new( name: "ColB" );
        public AVFXCurveColor ColorCenter = new( name: "ColC" );
        public AVFXCurveColor ColorEnd = new( name: "ColE" );
        public AVFXCurveColor ColorEdgeBegin = new( name: "CoEB" );
        public AVFXCurveColor ColorEdgeCenter = new( name: "CoEC" );
        public AVFXCurveColor ColorEdgeEnd = new( name: "CoEE" );
        private readonly List<Base> Attributes;

        public AVFXParticleDataPolyline() : base( "Data" ) {
            Attributes = new List<Base>( new Base[]{
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
            } );
        }

        public override void Read( AVFXNode node ) {
            Assigned = true;
            ReadAVFX( Attributes, node );
        }

        public override void ToDefault() {
            Assigned = true;
            SetDefault( Attributes );
            SetUnAssigned( Width );
            SetUnAssigned( WidthRandom );
            SetUnAssigned( WidthBegin );
            SetUnAssigned( WidthCenter );
            SetUnAssigned( WidthEnd );
            SetUnAssigned( Length );
            SetUnAssigned( Softness );
            SetUnAssigned( ColorBegin );
            SetUnAssigned( ColorCenter );
            SetUnAssigned( ColorEnd );
            SetUnAssigned( ColorEdgeBegin );
            SetUnAssigned( ColorEdgeCenter );
            SetUnAssigned( ColorEdgeEnd );
        }

        public override AVFXNode ToAVFX() {
            var dataAvfx = new AVFXNode( "Data" );
            PutAVFX( dataAvfx, Attributes );
            return dataAvfx;
        }
    }
}
