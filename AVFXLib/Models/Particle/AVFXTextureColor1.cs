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
        public LiteralBool Enabled = new LiteralBool("enabled", "bEna");
        public LiteralBool ColorToAlpha = new LiteralBool("colorToAlpha", "bC2A");
        public LiteralBool UseScreenCopy = new LiteralBool("useScreenCopy", "bUSC");
        public LiteralBool PreviousFrameCopy = new LiteralBool("previousFrameCopy", "bPFC");
        public LiteralInt UvSetIdx = new LiteralInt("uvSetIdx", "UvSN");
        public LiteralEnum<TextureFilterType> TextureFilter = new LiteralEnum<TextureFilterType>("textureFilter", "TFT");
        public LiteralEnum<TextureBorderType> TextureBorderU = new LiteralEnum<TextureBorderType>("textureBorderU", "TBUT");
        public LiteralEnum<TextureBorderType> TextureBorderV = new LiteralEnum<TextureBorderType>("textureBorderV", "TBVT");
        public LiteralEnum<TextureCalculateColor> TextureCalculateColor = new LiteralEnum<TextureCalculateColor>("textureCalculateColor", "TCCT");
        public LiteralEnum<TextureCalculateAlpha> TextureCalculateAlpha = new LiteralEnum<TextureCalculateAlpha>("textureCalculateAlpha", "TCAT");
        public LiteralInt TextureIdx = new LiteralInt("textureIdx", "TxNo");
        public LiteralInt MaskTextureIdx = new LiteralInt("maskTextureIdx", "TLst");

        List<Base> Attributes;

        public AVFXTextureColor1(string jsonPath) : base(jsonPath, "TC1")
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
                MaskTextureIdx
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
        }

        public override JToken toJSON()
        {
            JObject elem = new JObject();
            PutJSON(elem, Attributes);
            return elem;
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("TC1");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
