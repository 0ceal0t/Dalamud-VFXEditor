using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXTextureDistortion : Base
    {
        public LiteralBool Enabled = new LiteralBool("enabled", "bEna");
        public LiteralBool TargetUV1 = new LiteralBool("targetUV1", "bT1");
        public LiteralBool TargetUV2 = new LiteralBool("targetUV2", "bT2");
        public LiteralBool TargetUV3 = new LiteralBool("targetUV3", "bT3");
        public LiteralBool TargetUV4 = new LiteralBool("targetUV4", "bT4");
        public LiteralInt UvSetIdx = new LiteralInt("uvSetIdx", "UvSN");
        public LiteralEnum<TextureFilterType> TextureFilter = new LiteralEnum<TextureFilterType>("textureFilter", "TFT");
        public LiteralEnum<TextureBorderType> TextureBorderU = new LiteralEnum<TextureBorderType>("textureBorderU", "TBUT");
        public LiteralEnum<TextureBorderType> TextureBorderV = new LiteralEnum<TextureBorderType>("textureBorderV", "TBVT");
        public LiteralInt TextureIdx = new LiteralInt("textureIdx", "TxNo");
        public AVFXCurve DPow = new AVFXCurve("distortPower", "DPow");

        List<Base> Attributes;

        public AVFXTextureDistortion(string jsonPath) : base(jsonPath, "TD")
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

        public override void read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
        }

        public override void toDefault()
        {
            Assigned = true;
            SetDefault(Attributes);
            SetUnAssigned(DPow);
        }

        public override JToken toJSON()
        {
            JObject elem = new JObject();
            PutJSON(elem, Attributes);
            return elem;
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("TD");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
