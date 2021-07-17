using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXParticleDataLightModel : AVFXParticleData
    {
        public LiteralInt ModelIdx = new LiteralInt("MNO", size: 1);

        List<Base> Attributes;

        public AVFXParticleDataLightModel() : base("Data")
        {
            Attributes = new List<Base>(new Base[]{
                ModelIdx
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
