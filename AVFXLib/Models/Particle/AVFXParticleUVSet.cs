using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXParticleUVSet : Base
    {
        public const string NAME = "UvSt";

        public LiteralEnum<TextureCalculateUV> CalculateUVType = new LiteralEnum<TextureCalculateUV>("calculateUV", "CUvT");
        public AVFXCurve2Axis Scale = new AVFXCurve2Axis("scale", "Scl");
        public AVFXCurve2Axis Scroll = new AVFXCurve2Axis("scroll", "Scr");
        public AVFXCurve Rot = new AVFXCurve("rotation", "Rot");
        public AVFXCurve RotRandom = new AVFXCurve("rotationRandom", "RotR");

        List<Base> Attributes;

        public AVFXParticleUVSet() : base("uvSet", NAME)
        {
            Attributes = new List<Base>(new Base[] {
                CalculateUVType,
                Scale,
                Scroll,
                Rot,
                RotRandom
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
            SetDefault(CalculateUVType);
        }

        public override JToken toJSON()
        {
            JObject elem = new JObject();
            PutJSON(elem, Attributes);
            return elem;
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode uvstAvfx = new AVFXNode("UvSt");
            PutAVFX(uvstAvfx, Attributes);
            return uvstAvfx;
        }
    }
}
