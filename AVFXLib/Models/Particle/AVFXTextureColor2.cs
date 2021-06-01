using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXTextureColor2 : Base
    {
        public LiteralBool Enabled = new LiteralBool("bEna");
        public LiteralBool ColorToAlpha = new LiteralBool("bC2A");
        public LiteralBool UseScreenCopy = new LiteralBool("bUSC");
        public LiteralBool PreviousFrameCopy = new LiteralBool("bPFC");
        public LiteralInt UvSetIdx = new LiteralInt("UvSN");
        public LiteralEnum<TextureFilterType> TextureFilter = new LiteralEnum<TextureFilterType>("TFT");
        public LiteralEnum<TextureBorderType> TextureBorderU = new LiteralEnum<TextureBorderType>("TBUT");
        public LiteralEnum<TextureBorderType> TextureBorderV = new LiteralEnum<TextureBorderType>("TBVT");
        public LiteralEnum<TextureCalculateColor> TextureCalculateColor = new LiteralEnum<TextureCalculateColor>("TCCT");
        public LiteralEnum<TextureCalculateAlpha> TextureCalculateAlpha = new LiteralEnum<TextureCalculateAlpha>("TCAT");
        public LiteralInt TextureIdx = new LiteralInt("TxNo");

        List<Base> Attributes;

        public AVFXTextureColor2(string avfxName) : base(avfxName)
        {
            Attributes = new List<Base>(new Base[]{
                Enabled,
                ColorToAlpha,
                UseScreenCopy,
                PreviousFrameCopy,
                UvSetIdx,
                TextureFilter,
                TextureBorderU,
                TextureBorderV,
                TextureCalculateColor,
                TextureCalculateAlpha,
                TextureIdx
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
        }

        public override AVFXNode ToAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode(AVFXName);
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
