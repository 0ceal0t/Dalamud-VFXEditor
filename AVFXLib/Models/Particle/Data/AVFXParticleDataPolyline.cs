using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXParticleDataPolyline : AVFXParticleData
    {
        public LiteralEnum<LineCreateType> CreateLineType = new LiteralEnum<LineCreateType>("LnCT");
        public LiteralEnum<NotBillboardBaseAxisType> NotBillBoardBaseAxisType = new LiteralEnum<NotBillboardBaseAxisType>("NBBA");
        public LiteralInt BindWeaponType = new LiteralInt("BWpT");
        public LiteralInt PointCount = new LiteralInt("PnC");
        public LiteralInt PointCountCenter = new LiteralInt("PnCC");
        public LiteralInt PointCountEndDistortion = new LiteralInt("PnED");
        public LiteralBool UseEdge = new LiteralBool("bEdg");
        public LiteralBool NotBillboard = new LiteralBool("bNtB");
        public LiteralBool BindWeapon = new LiteralBool("BdWp");
        public LiteralBool ConnectTarget = new LiteralBool("bCtg");
        public LiteralBool ConnectTargetReverse = new LiteralBool("bCtr");
        public LiteralInt TagNumber = new LiteralInt("TagN");
        public LiteralBool IsSpline = new LiteralBool("bSpl");
        public LiteralBool IsLocal = new LiteralBool("bLcl");

        public AVFXCurve CF = new AVFXCurve( "CF" );
        public AVFXCurve Width = new AVFXCurve("Wd");
        public AVFXCurve WidthRandom = new AVFXCurve("WdR");
        public AVFXCurve WidthBegin = new AVFXCurve("WdB");
        public AVFXCurve WidthCenter = new AVFXCurve("WdC");
        public AVFXCurve WidthEnd = new AVFXCurve("WdE");
        public AVFXCurve Length = new AVFXCurve("Len");
        public AVFXCurve LengthRandom = new AVFXCurve( "LenR" );
        public AVFXCurve Softness = new AVFXCurve("Sft");
        public AVFXCurve SoftRandom = new AVFXCurve( "SftR" );
        public AVFXCurve PnDs = new AVFXCurve( "PnDs" );
        public AVFXCurveColor ColorBegin = new AVFXCurveColor(name: "ColB");
        public AVFXCurveColor ColorCenter = new AVFXCurveColor(name: "ColC");
        public AVFXCurveColor ColorEnd = new AVFXCurveColor(name: "ColE");
        public AVFXCurveColor ColorEdgeBegin = new AVFXCurveColor(name: "CoEB");
        public AVFXCurveColor ColorEdgeCenter = new AVFXCurveColor(name: "CoEC");
        public AVFXCurveColor ColorEdgeEnd = new AVFXCurveColor(name: "CoEE");

        List<Base> Attributes;

        public AVFXParticleDataPolyline() : base("Data")
        {
            Attributes = new List<Base>(new Base[]{
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
            });
        }

        public override void read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
        }

        public override void toDefault()
        {
            Assigned = true;
            SetDefault(Attributes);
            SetUnAssigned(Width);
            SetUnAssigned(WidthRandom);
            SetUnAssigned(WidthBegin);
            SetUnAssigned(WidthCenter);
            SetUnAssigned(WidthEnd);
            SetUnAssigned(Length);
            SetUnAssigned(Softness);
            SetUnAssigned(ColorBegin);
            SetUnAssigned(ColorCenter);
            SetUnAssigned(ColorEnd);
            SetUnAssigned(ColorEdgeBegin);
            SetUnAssigned(ColorEdgeCenter);
            SetUnAssigned(ColorEdgeEnd);
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
