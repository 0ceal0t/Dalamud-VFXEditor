using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXBinderDataPoint : AVFXBinderData
    {
        public AVFXCurve SpringStrength = new AVFXCurve("SpS");

        List<Base> Attributes;

        public AVFXBinderDataPoint() : base("Data")
        {
            Attributes = new List<Base>(new Base[]{
                SpringStrength
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
            SpringStrength.toDefault();
            SpringStrength.addKey();
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
