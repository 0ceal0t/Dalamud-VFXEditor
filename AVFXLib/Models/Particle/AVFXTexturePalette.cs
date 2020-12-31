using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXTexturePalette : Base
    {
        public LiteralBool Enabled = new LiteralBool("enabled", "bEna");
        public LiteralEnum<TextureFilterType> TextureFilter = new LiteralEnum<TextureFilterType>("textureFilter", "TFT");
        public LiteralEnum<TextureBorderType> TextureBorder = new LiteralEnum<TextureBorderType>("textureBorder", "TBT");
        public LiteralInt TextureIdx = new LiteralInt("textureIdx", "TxNo");

        List<Base> Attributes;

        public AVFXTexturePalette(string jsonPath) : base(jsonPath, "TP")
        {
            Attributes = new List<Base>(new Base[]{
                Enabled,
                TextureFilter,
                TextureBorder,
                TextureIdx
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
            AVFXNode dataAvfx = new AVFXNode("TP");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
