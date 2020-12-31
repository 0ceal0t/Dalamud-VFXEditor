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
        public LiteralBool Enabled = new LiteralBool("enabled", "bEna");
        public LiteralInt UvSetIdx = new LiteralInt("uvSetIdx", "UvSN");
        public LiteralEnum<TextureFilterType> TextureFilter = new LiteralEnum<TextureFilterType>("textureFilter", "TFT");
        public LiteralEnum<TextureBorderType> TextureBorderU = new LiteralEnum<TextureBorderType>("textureBorderU", "TBUT");
        public LiteralEnum<TextureBorderType> TextureBorderV = new LiteralEnum<TextureBorderType>("textureBorderV", "TBVT");
        public LiteralInt TextureIdx = new LiteralInt("textureIdx", "TxNo");
        public AVFXCurve NPow = new AVFXCurve("normalPower", "NPow");

        List<Base> Attributes;

        public AVFXTextureNormal(string jsonPath) : base(jsonPath, "TN")
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

        public override void read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
        }

        public override void toDefault()
        {
            Assigned = true;
            SetDefault(Attributes);
            SetUnAssigned(NPow);
        }

        public override JToken toJSON()
        {
            JObject elem = new JObject();
            PutJSON(elem, Attributes);
            return elem;
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("TN");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
