using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXParticleDataDisc : AVFXParticleData
    {
        public LiteralInt PartsCount = new LiteralInt("PrtC");
        public LiteralInt PartsCountU = new LiteralInt("PCnU");
        public LiteralInt PartsCountV = new LiteralInt("PCnV");
        public LiteralFloat PointIntervalFactoryV = new LiteralFloat("PIFU");

        public AVFXCurve Angle = new AVFXCurve("Ang");
        public AVFXCurve HeightBeginInner = new AVFXCurve( "HBI" );
        public AVFXCurve HeightEndInner = new AVFXCurve( "HEI" );
        public AVFXCurve HeightBeginOuter = new AVFXCurve( "HBO" );
        public AVFXCurve HeightEndOuter = new AVFXCurve( "HEO" );
        public AVFXCurve WidthBegin = new AVFXCurve("WB");
        public AVFXCurve WidthEnd = new AVFXCurve("WE");
        public AVFXCurve RadiusBegin = new AVFXCurve("RB");
        public AVFXCurve RadiusEnd = new AVFXCurve( "RE");
        public AVFXCurveColor ColorEdgeInner = new AVFXCurveColor(name:"CEI");
        public AVFXCurveColor ColorEdgeOuter = new AVFXCurveColor(name: "CEO");

        List<Base> Attributes;

        public AVFXParticleDataDisc() : base("Data")
        {
            Attributes = new List<Base>(new Base[]{
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
            });
        }

        public override void Read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
        }

        public override void ToDefault()
        {
            Assigned = true;
            SetDefault(Attributes);
            SetUnAssigned(Angle);
            SetUnAssigned(WidthBegin);
            SetUnAssigned(WidthEnd);
            SetUnAssigned(RadiusBegin);
            SetUnAssigned(RadiusEnd);
            SetUnAssigned(ColorEdgeInner);
            SetUnAssigned(ColorEdgeOuter);
        }

        public override AVFXNode ToAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
