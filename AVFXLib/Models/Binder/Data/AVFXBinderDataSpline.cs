using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXBinderDataSpline : AVFXBinderData
    {
        public AVFXCurve CarryOverFactor = new AVFXCurve("carryOverFactor", "COF");
        public AVFXCurve CarryOverFactorRandom = new AVFXCurve("carryOverFactorRandom", "COFR");

        List<Base> Attributes;

        public AVFXBinderDataSpline(string jsonPath) : base(jsonPath, "Data")
        {
            Attributes = new List<Base>(new Base[]{
                CarryOverFactor,
                CarryOverFactorRandom
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
