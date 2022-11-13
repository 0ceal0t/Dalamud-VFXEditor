using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleDataPolyline : AvfxData {
        public readonly AvfxEnum<LineCreateType> CreateLineType = new( "Create Line Type", "LnCT" );
        public readonly AvfxEnum<NotBillboardBaseAxisType> NotBillBoardBaseAxisType = new( "Not Billboard Base Axis", "NBBA" );
        public readonly AvfxInt BindWeaponType = new( "Bind Weapon Type", "BWpT" );
        public readonly AvfxInt PointCount = new( "Point Count", "PnC" );
        public readonly AvfxInt PointCountCenter = new( "Pount Count Center", "PnCC" );
        public readonly AvfxInt PointCountEndDistortion = new( "Point Count End Distortion", "PnED" );
        public readonly AvfxBool UseEdge = new( "Use Edge", "bEdg" );
        public readonly AvfxBool NotBillboard = new( "No Billboard", "bNtB" );
        public readonly AvfxBool BindWeapon = new( "Bind Weapon", "BdWp" );
        public readonly AvfxBool ConnectTarget = new( "Connect Target", "bCtg" );
        public readonly AvfxBool ConnectTargetReverse = new( "Connect Target Reverse", "bCtr" );
        public readonly AvfxInt TagNumber = new( "Tag Number", "TagN" );
        public readonly AvfxBool IsSpline = new( "Is Spline", "bSpl" );
        public readonly AvfxBool IsLocal = new( "Is Local", "bLcl" );

        public readonly AvfxCurve CF = new( "CF (Unknown)", "CF" );
        public readonly AvfxCurve Width = new( "Width", "Wd" );
        public readonly AvfxCurve WidthRandom = new( "Width Random", "WdR" );
        public readonly AvfxCurve WidthBegin = new( "Width Begin", "WdB" );
        public readonly AvfxCurve WidthCenter = new( "Width Center", "WdC" );
        public readonly AvfxCurve WidthEnd = new( "Width End", "WdE" );
        public readonly AvfxCurve Length = new( "Length", "Len" );
        public readonly AvfxCurve LengthRandom = new( "Length Random", "LenR" );
        public readonly AvfxCurve Softness = new( "Softness", "Sft" );
        public readonly AvfxCurve SoftRandom = new( "Softness Random", "SftR" );
        public readonly AvfxCurve PnDs = new( "PnDs (Unknown)", "PnDs" );
        public readonly AvfxCurveColor ColorBegin = new( name: "Color Begin", "ColB" );
        public readonly AvfxCurveColor ColorCenter = new( name: "Color Center", "ColC" );
        public readonly AvfxCurveColor ColorEnd = new( name: "Color End", "ColE" );
        public readonly AvfxCurveColor ColorEdgeBegin = new( name: "Color Edge Begin", "CoEB" );
        public readonly AvfxCurveColor ColorEdgeCenter = new( name: "Color Edge Center", "CoEC" );
        public readonly AvfxCurveColor ColorEdgeEnd = new( name: "Color Edge End", "CoEE" );

        public readonly UiParameters Parameters;

        public AvfxParticleDataPolyline() : base() {
            Children = new() {
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

            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( CreateLineType );
            Parameters.Add( NotBillBoardBaseAxisType );
            Parameters.Add( BindWeaponType );
            Parameters.Add( PointCount );
            Parameters.Add( PointCountCenter );
            Parameters.Add( PointCountEndDistortion );
            Parameters.Add( UseEdge );
            Parameters.Add( NotBillboard );
            Parameters.Add( BindWeapon );
            Parameters.Add( ConnectTarget );
            Parameters.Add( ConnectTargetReverse );
            Parameters.Add( TagNumber );
            Parameters.Add( IsSpline );
            Parameters.Add( IsLocal );

            Tabs.Add( Width );
            Tabs.Add( WidthBegin );
            Tabs.Add( WidthCenter );
            Tabs.Add( WidthEnd );
            Tabs.Add( Length );
            Tabs.Add( LengthRandom );

            Tabs.Add( ColorBegin );
            Tabs.Add( ColorCenter );
            Tabs.Add( ColorEnd );
            Tabs.Add( ColorEdgeBegin );
            Tabs.Add( ColorEdgeCenter );
            Tabs.Add( ColorEdgeEnd );

            Tabs.Add( CF );
            Tabs.Add( Softness );
            Tabs.Add( SoftRandom );
            Tabs.Add( PnDs );
        }
    }
}
