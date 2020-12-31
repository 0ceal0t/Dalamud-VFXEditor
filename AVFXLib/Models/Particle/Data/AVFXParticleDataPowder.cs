using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXParticleDataPowder : AVFXParticleData
    {
        public LiteralBool IsLightning = new LiteralBool("isLightning", "bLgt");
        public LiteralEnum<DirectionalLightType> DirectionalLightType = new LiteralEnum<DirectionalLightType>("directionalLightType", "LgtT");
        public LiteralFloat CenterOffset = new LiteralFloat("centerOffset", "CnOf");

        List<Base> Attributes;

        public AVFXParticleDataPowder(string jsonPath) : base(jsonPath, "Data")
        {
            Attributes = new List<Base>(new Base[]{
                IsLightning,
                DirectionalLightType,
                CenterOffset
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
            AVFXNode dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
