using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXBinderDataSpline : AVFXBinderData
    {
        public AVFXCurve CarryOverFactor = new AVFXCurve("COF");
        public AVFXCurve CarryOverFactorRandom = new AVFXCurve("COFR");

        List<Base> Attributes;

        public AVFXBinderDataSpline() : base("Data")
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
            AVFXNode dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
