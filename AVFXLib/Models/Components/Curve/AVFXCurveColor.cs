using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXCurveColor : Base
    {

        public AVFXCurve RGB = new AVFXCurve("RGB", "RGB");
        public AVFXCurve A = new AVFXCurve("alpha", "A");
        public AVFXCurve SclR = new AVFXCurve("scaleR", "SclR");
        public AVFXCurve SclG = new AVFXCurve("scaleG", "SclG");
        public AVFXCurve SclB = new AVFXCurve("scaleB", "SclB");
        public AVFXCurve SclA = new AVFXCurve("scaleAlpha", "SclA");
        public AVFXCurve Bri = new AVFXCurve("brightness", "Bri");
        public AVFXCurve RanR = new AVFXCurve("randomR", "RanR");
        public AVFXCurve RanG = new AVFXCurve("randomB", "RanG");
        public AVFXCurve RanB = new AVFXCurve("randomG", "RanB");
        public AVFXCurve RanA = new AVFXCurve("randomAlpha", "RanA");
        public AVFXCurve RBri = new AVFXCurve("randomBrightness", "RBri");

        List<Base> Attributes;

        public AVFXCurveColor(string jsonPath, string name = "Col") : base(jsonPath, name)
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

        public override void read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
        }

        public override void toDefault()
        {
            Assigned = true;
            SetUnAssigned(Attributes);
        }

        public override JToken toJSON()
        {
            JObject elem = new JObject();
            PutJSON(elem, Attributes);
            return elem;
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode curveAvfx = new AVFXNode(AVFXName);
            PutAVFX(curveAvfx, Attributes);
            return curveAvfx;
        }
    }
}
