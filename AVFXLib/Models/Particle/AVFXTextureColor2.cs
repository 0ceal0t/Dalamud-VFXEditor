using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXTextureColor2 : Base
    {
        public LiteralBool Enabled = new("bEna");
        public LiteralBool ColorToAlpha = new("bC2A");
        public LiteralBool UseScreenCopy = new("bUSC");
        public LiteralBool PreviousFrameCopy = new("bPFC");
        public LiteralInt UvSetIdx = new("UvSN");
        public LiteralEnum<TextureFilterType> TextureFilter = new("TFT");
        public LiteralEnum<TextureBorderType> TextureBorderU = new("TBUT");
        public LiteralEnum<TextureBorderType> TextureBorderV = new("TBVT");
        public LiteralEnum<TextureCalculateColor> TextureCalculateColor = new("TCCT");
        public LiteralEnum<TextureCalculateAlpha> TextureCalculateAlpha = new("TCAT");
        public LiteralInt TextureIdx = new("TxNo");
        readonly List<Base> Attributes;

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
            var dataAvfx = new AVFXNode(AVFXName);
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
