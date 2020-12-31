using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXCurve2Axis : Base
    {
        public LiteralEnum<AxisConnect> AxisConnectType = new LiteralEnum<AxisConnect>("axisConnectType", "ACT");
        public LiteralEnum<RandomType> AxisConnectRandomType = new LiteralEnum<RandomType>("axisConnectRandomType", "ACTR");
        public AVFXCurve X = new AVFXCurve("X", "X");
        public AVFXCurve Y = new AVFXCurve("Y", "Y");
        public AVFXCurve RX = new AVFXCurve("RandomX", "XR");
        public AVFXCurve RY = new AVFXCurve("RandomY", "YR");

        List<Base> Attributes;

        public AVFXCurve2Axis(string jsonPath, string avfxName) : base(jsonPath, avfxName)
        {
            Attributes = new List<Base>(new Base[]{
                AxisConnectType,
                AxisConnectRandomType,
                X,
                Y,
                RX,
                RY,
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
            SetUnAssigned(Attributes);
            SetDefault(AxisConnectType);
            SetDefault(AxisConnectRandomType);
        }

        public override JToken toJSON()
        {
            JObject elem = new JObject();
            PutJSON(elem, Attributes);
            return elem;
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode curveAvfx = new AVFXNode(AVFXName);
            PutAVFX(curveAvfx, Attributes);
            return curveAvfx;
        }
    }
}
