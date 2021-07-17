using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXParticleDataLaser : AVFXParticleData
    {
        public AVFXCurve Length = new AVFXCurve("Len");
        public AVFXCurve Width = new AVFXCurve("Wdt");

        List<Base> Attributes;

        public AVFXParticleDataLaser() : base("Data")
        {
            Attributes = new List<Base>(new Base[]{
                Length,
                Width
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
