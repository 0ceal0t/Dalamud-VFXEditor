using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXParticleDataDecalRing : AVFXParticleData
    {
        public AVFXCurve Width = new AVFXCurve("width", "WID");
        public LiteralFloat ScalingScale = new LiteralFloat("scalingScale", "SS");
        public LiteralFloat RingFan = new LiteralFloat("ringFan", "RF");

        List<Base> Attributes;

        public AVFXParticleDataDecalRing(string jsonPath) : base(jsonPath, "Data")
        {
            Attributes = new List<Base>(new Base[]{
                Width,
                ScalingScale,
                RingFan
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
            SetUnAssigned(ScalingScale);
            SetUnAssigned(RingFan);
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
