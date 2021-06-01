using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXParticleDataWindmill : AVFXParticleData
    {
        public LiteralEnum<WindmillUVType> WindmillUVType = new LiteralEnum<WindmillUVType>("WUvT");

        List<Base> Attributes;

        public AVFXParticleDataWindmill() : base("Data")
        {
            Attributes = new List<Base>(new Base[]{
                WindmillUVType
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
        }

        public override AVFXNode ToAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
