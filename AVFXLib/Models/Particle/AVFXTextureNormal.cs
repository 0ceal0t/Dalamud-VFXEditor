using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXTextureNormal : Base
    {
        public LiteralBool Enabled = new LiteralBool("bEna");
        public LiteralInt UvSetIdx = new LiteralInt("UvSN");
        public LiteralEnum<TextureFilterType> TextureFilter = new LiteralEnum<TextureFilterType>("TFT");
        public LiteralEnum<TextureBorderType> TextureBorderU = new LiteralEnum<TextureBorderType>("TBUT");
        public LiteralEnum<TextureBorderType> TextureBorderV = new LiteralEnum<TextureBorderType>("TBVT");
        public LiteralInt TextureIdx = new LiteralInt("TxNo");
        public AVFXCurve NPow = new AVFXCurve("NPow");

        List<Base> Attributes;

        public AVFXTextureNormal() : base("TN")
        {
            Attributes = new List<Base>(new Base[]{
                Enabled,
                UvSetIdx,
                TextureFilter,
                TextureBorderU,
                TextureBorderV,
                TextureIdx,
                NPow
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
            SetDefault( NPow );
            NPow.AddKey();
            TextureIdx.GiveValue( -1 );
        }

        public override AVFXNode ToAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("TN");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
