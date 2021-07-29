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
        public LiteralBool Enabled = new("bEna");
        public LiteralBool TargetUV1 = new("bT1");
        public LiteralBool TargetUV2 = new("bT2");
        public LiteralBool TargetUV3 = new("bT3");
        public LiteralBool TargetUV4 = new("bT4");
        public LiteralInt UvSetIdx = new("UvSN");
        public LiteralEnum<TextureFilterType> TextureFilter = new("TFT");
        public LiteralEnum<TextureBorderType> TextureBorderU = new("TBUT");
        public LiteralEnum<TextureBorderType> TextureBorderV = new("TBVT");
        public LiteralInt TextureIdx = new("TxNo");
        public AVFXCurve DPow = new("DPow");
        readonly List<Base> Attributes;

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
            var dataAvfx = new AVFXNode("TD");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
