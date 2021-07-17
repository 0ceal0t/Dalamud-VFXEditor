using AVFXLib.AVFX;
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

        public LiteralEnum<TextureCalculateUV> CalculateUVType = new LiteralEnum<TextureCalculateUV>("CUvT");
        public AVFXCurve2Axis Scale = new AVFXCurve2Axis("Scl");
        public AVFXCurve2Axis Scroll = new AVFXCurve2Axis("Scr");
        public AVFXCurve Rot = new AVFXCurve("Rot");
        public AVFXCurve RotRandom = new AVFXCurve("RotR");

        List<Base> Attributes;

        public AVFXParticleUVSet() : base(NAME)
        {
            Attributes = new List<Base>(new Base[] {
                CalculateUVType,
                Scale,
                Scroll,
                Rot,
                RotRandom
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
            SetDefault(CalculateUVType);
        }

        public override AVFXNode ToAVFX()
        {
            AVFXNode uvstAvfx = new AVFXNode("UvSt");
            PutAVFX(uvstAvfx, Attributes);
            return uvstAvfx;
        }
    }
}
