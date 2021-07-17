using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXCurveColor : Base
    {

        public AVFXCurve RGB = new AVFXCurve("RGB");
        public AVFXCurve A = new AVFXCurve("A");
        public AVFXCurve SclR = new AVFXCurve("SclR");
        public AVFXCurve SclG = new AVFXCurve("SclG");
        public AVFXCurve SclB = new AVFXCurve("SclB");
        public AVFXCurve SclA = new AVFXCurve("SclA");
        public AVFXCurve Bri = new AVFXCurve("Bri");
        public AVFXCurve RanR = new AVFXCurve("RanR");
        public AVFXCurve RanG = new AVFXCurve("RanG");
        public AVFXCurve RanB = new AVFXCurve("RanB");
        public AVFXCurve RanA = new AVFXCurve("RanA");
        public AVFXCurve RBri = new AVFXCurve("RBri");

        List<Base> Attributes;

        public AVFXCurveColor(string name = "Col") : base(name)
        {
            Attributes = new List<Base>(new Base[] {
                RGB,
                A,
                SclR,
                SclG,
                SclB,
                SclA,
                Bri,
                RanR,
                RanG,
                RanB,
                RanA,
                RBri
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
            AVFXNode curveAvfx = new AVFXNode(AVFXName);
            PutAVFX(curveAvfx, Attributes);
            return curveAvfx;
        }
    }
}
