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

        public AVFXCurve RGB = new("RGB");
        public AVFXCurve A = new("A");
        public AVFXCurve SclR = new("SclR");
        public AVFXCurve SclG = new("SclG");
        public AVFXCurve SclB = new("SclB");
        public AVFXCurve SclA = new("SclA");
        public AVFXCurve Bri = new("Bri");
        public AVFXCurve RanR = new("RanR");
        public AVFXCurve RanG = new("RanG");
        public AVFXCurve RanB = new("RanB");
        public AVFXCurve RanA = new("RanA");
        public AVFXCurve RBri = new("RBri");
        readonly List<Base> Attributes;

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
            var curveAvfx = new AVFXNode(AVFXName);
            PutAVFX(curveAvfx, Attributes);
            return curveAvfx;
        }
    }
}
