using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXEffectorDataRadialBlur : AVFXEffectorData
    {
        public LiteralFloat FadeStartDistance = new LiteralFloat("fadeStartDistance", "FSDc");
        public LiteralFloat FadeEndDistance = new LiteralFloat("fadeEndDistance", "FEDc");
        public LiteralEnum<ClipBasePoint> FadeBasePointType = new LiteralEnum<ClipBasePoint>("fadeBasePoint", "FaBP");
        public AVFXCurve Length = new AVFXCurve("length", "Len");
        public AVFXCurve Strength = new AVFXCurve("strength", "Str");
        public AVFXCurve Gradation = new AVFXCurve("gradation", "Gra");
        public AVFXCurve InnerRadius = new AVFXCurve("innerRadius", "IRad");
        public AVFXCurve OuterRadius = new AVFXCurve("outerRadius", "ORad");

        List<Base> Attributes;

        public AVFXEffectorDataRadialBlur(string jsonPath) : base(jsonPath, "Data")
        {
            Attributes = new List<Base>(new Base[]{
                FadeStartDistance,
                FadeEndDistance,
                FadeBasePointType,
                Length,
                Strength,
                Gradation,
                InnerRadius,
                OuterRadius
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
            SetDefault(Attributes);
            SetUnAssigned(Length);
            SetUnAssigned(Strength);
            SetUnAssigned(Gradation);
            SetUnAssigned(InnerRadius);
            SetUnAssigned(OuterRadius);
        }

        public override JToken toJSON()
        {
            JObject elem = new JObject();
            PutJSON(elem, Attributes);
            return elem;
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
