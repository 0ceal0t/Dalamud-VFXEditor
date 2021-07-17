using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXParticleDataDecal : AVFXParticleData
    {
        public LiteralFloat ScalingScale = new LiteralFloat("SS");

        List<Base> Attributes;

        public AVFXParticleDataDecal() : base("Data")
        {
            Attributes = new List<Base>(new Base[]{
                ScalingScale
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
