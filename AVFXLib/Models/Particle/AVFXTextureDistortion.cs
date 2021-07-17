using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXTextureDistortion : Base
    {
        public LiteralBool Enabled = new LiteralBool("bEna");
        public LiteralBool TargetUV1 = new LiteralBool("bT1");
        public LiteralBool TargetUV2 = new LiteralBool("bT2");
        public LiteralBool TargetUV3 = new LiteralBool("bT3");
        public LiteralBool TargetUV4 = new LiteralBool("bT4");
        public LiteralInt UvSetIdx = new LiteralInt("UvSN");
        public LiteralEnum<TextureFilterType> TextureFilter = new LiteralEnum<TextureFilterType>("TFT");
        public LiteralEnum<TextureBorderType> TextureBorderU = new LiteralEnum<TextureBorderType>("TBUT");
        public LiteralEnum<TextureBorderType> TextureBorderV = new LiteralEnum<TextureBorderType>("TBVT");
        public LiteralInt TextureIdx = new LiteralInt("TxNo");
        public AVFXCurve DPow = new AVFXCurve("DPow");

        List<Base> Attributes;

        public AVFXTextureDistortion() : base("TD")
        {
            Attributes = new List<Base>(new Base[]{
                Enabled,
                TargetUV1,
                TargetUV2,
                TargetUV3,
                TargetUV4,
                UvSetIdx,
                TextureFilter,
                TextureBorderU,
                TextureBorderV,
                TextureIdx,
                DPow
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
            DPow.ToDefault();
            DPow.AddKey();

        }

        public override AVFXNode ToAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("TD");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
