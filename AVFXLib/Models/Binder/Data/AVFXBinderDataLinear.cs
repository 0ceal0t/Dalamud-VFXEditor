using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXBinderDataLinear : AVFXBinderData
    {
        public AVFXCurve CarryOverFactor = new AVFXCurve("COF");
        public AVFXCurve CarryOverFactorRandom = new AVFXCurve("COFR");

        List<Base> Attributes;

        public AVFXBinderDataLinear() : base("Data")
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

        public override AVFXNode toAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
