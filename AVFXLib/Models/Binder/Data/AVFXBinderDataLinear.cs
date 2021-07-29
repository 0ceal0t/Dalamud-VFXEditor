using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXBinderDataLinear : AVFXBinderData
    {
        public AVFXCurve CarryOverFactor = new("COF");
        public AVFXCurve CarryOverFactorRandom = new("COFR");
        readonly List<Base> Attributes;

        public AVFXBinderDataLinear() : base("Data")
        {
            Attributes = new List<Base>(new Base[]{
                CarryOverFactor,
                CarryOverFactorRandom
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
            SetUnAssigned(Attributes);
        }

        public override AVFXNode ToAVFX()
        {
            var dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
