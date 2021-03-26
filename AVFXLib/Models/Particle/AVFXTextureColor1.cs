using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXTextureColor1 : Base
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
        public LiteralIntList MaskTextureIdx = new LiteralIntList( "TLst");
        public AVFXCurve TexN = new AVFXCurve( "TxN" );
        public AVFXCurve TexNRandom = new AVFXCurve( "TxNR" );

        List<Base> Attributes;

        public AVFXTextureColor1() : base("TC1")
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
                TextureIdx,
                MaskTextureIdx,
                TexN,
                TexNRandom
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
            SetUnAssigned( TexN );
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("TC1");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
