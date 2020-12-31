using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXParticleDataModel : AVFXParticleData
    {
        public LiteralInt ModelNumberRandomValue = new LiteralInt("modelNumberRandomValue", "MNRv");
        public LiteralEnum<RandomType> ModelNumberRandomType = new LiteralEnum<RandomType>("modelNumberRandomType", "MNRt");
        public LiteralInt ModelNumberRandomInterval = new LiteralInt("modelNumberRandomInterval", "MNRi");
        public LiteralEnum<FresnelType> FresnelType = new LiteralEnum<FresnelType>("fresnelType", "FrsT");
        public LiteralEnum<DirectionalLightType> DirectionalLightType = new LiteralEnum<DirectionalLightType>("directionalLightType", "DLT");
        public LiteralEnum<PointLightType> PointLightType = new LiteralEnum<PointLightType>("pointLightType", "PLT");
        public LiteralBool IsLightning = new LiteralBool("isLightning", "bLgt");
        public LiteralBool IsMorph = new LiteralBool("isMorph", "bShp");
        public LiteralInt ModelIdx = new LiteralInt("modelIdx", "MdNo", size: 1);

        public AVFXCurve Morph = new AVFXCurve("morph", "Moph");
        public AVFXCurve FresnelCurve = new AVFXCurve("fresnelCurve", "FrC");
        public AVFXCurve3Axis FresnelRotation = new AVFXCurve3Axis("fresnelRotation", "FrRt");
        public AVFXCurveColor ColorBegin = new AVFXCurveColor("colorBegin", name: "ColB");
        public AVFXCurveColor ColorEnd = new AVFXCurveColor("colorEnd", name: "ColE");

        List<Base> Attributes;

        public AVFXParticleDataModel(string jsonPath) : base(jsonPath, "Data")
        {
            Attributes = new List<Base>(new Base[]{
                ModelNumberRandomValue,
                ModelNumberRandomType,
                ModelNumberRandomInterval,
                FresnelType,
                DirectionalLightType,
                PointLightType,
                IsLightning,
                IsMorph,
                ModelIdx,
                Morph,
                FresnelCurve,
                FresnelRotation,
                ColorBegin,
                ColorEnd
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
            SetUnAssigned(Morph);
            SetUnAssigned(FresnelCurve);
            SetUnAssigned(FresnelRotation);
            SetUnAssigned(ColorBegin);
            SetUnAssigned(ColorEnd);
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
