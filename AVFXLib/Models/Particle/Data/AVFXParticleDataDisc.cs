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
        public LiteralInt PartsCount = new LiteralInt("partsCount", "PrtC");
        public LiteralInt PartsCountU = new LiteralInt("partsCountU", "PCnU");
        public LiteralInt PartsCountV = new LiteralInt("partsCountV", "PCnV");
        public LiteralFloat PointIntervalFactoryV = new LiteralFloat("pointIntervalFactorV", "PIFU");

        public AVFXCurve Angle = new AVFXCurve("angle", "Ang");
        public AVFXCurve WidthBegin = new AVFXCurve("widthAngle", "WB");
        public AVFXCurve WidthEnd = new AVFXCurve("widthEnd", "WE");
        public AVFXCurve RadiusBegin = new AVFXCurve("radiusBegin", "RB");
        public AVFXCurve RadiusEnd = new AVFXCurve("radiusEnd", "RE");
        public AVFXCurveColor ColorEdgeInner = new AVFXCurveColor("colorEdgeInner", name:"CEI");
        public AVFXCurveColor ColorEdgeOuter = new AVFXCurveColor("colorEdgeOuter", name: "CEO");

        List<Base> Attributes;

        public AVFXParticleDataDisc(string jsonPath) : base(jsonPath, "Data")
        {
            Attributes = new List<Base>(new Base[]{
                PartsCount,
                PartsCountU,
                PartsCountV,
                PointIntervalFactoryV,
                Angle,
                WidthBegin,
                WidthEnd,
                RadiusBegin,
                RadiusEnd,
                ColorEdgeInner,
                ColorEdgeOuter
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
            SetUnAssigned(Angle);
            SetUnAssigned(WidthBegin);
            SetUnAssigned(WidthEnd);
            SetUnAssigned(RadiusBegin);
            SetUnAssigned(RadiusEnd);
            SetUnAssigned(ColorEdgeInner);
            SetUnAssigned(ColorEdgeOuter);
        }

        public override JToken toJSON()
        {
            JObject elem = new JObject();
            PutJSON(elem, Attributes);
            return elem;
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
