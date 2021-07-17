using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXBinderDataCamera : AVFXBinderData
    {
        public AVFXCurve Distance = new AVFXCurve("Dst");
        public AVFXCurve DistanceRandom = new AVFXCurve( "DstR" );

        List<Base> Attributes;

        public AVFXBinderDataCamera() : base("Data")
        {
            Attributes = new List<Base>(new Base[]{
                Distance,
                DistanceRandom
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
