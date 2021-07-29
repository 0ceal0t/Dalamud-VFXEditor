using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXParticleDataDecalRing : AVFXParticleData
    {
        public AVFXCurve Width = new("WID");
        public LiteralFloat ScalingScale = new("SS");
        public LiteralFloat RingFan = new("RF");
        readonly List<Base> Attributes;

        public AVFXParticleDataDecalRing() : base("Data")
        {
            Attributes = new List<Base>(new Base[]{
                Width,
                ScalingScale,
                RingFan
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
            SetUnAssigned(ScalingScale);
            SetUnAssigned(RingFan);
        }

        public override AVFXNode ToAVFX()
        {
            var dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
