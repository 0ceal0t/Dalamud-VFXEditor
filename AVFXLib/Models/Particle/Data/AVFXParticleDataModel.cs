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
        public LiteralInt ModelNumberRandomValue = new LiteralInt("MNRv");
        public LiteralEnum<RandomType> ModelNumberRandomType = new LiteralEnum<RandomType>("MNRt");
        public LiteralInt ModelNumberRandomInterval = new LiteralInt("MNRi");
        public LiteralEnum<FresnelType> FresnelType = new LiteralEnum<FresnelType>("FrsT");
        public LiteralEnum<DirectionalLightType> DirectionalLightType = new LiteralEnum<DirectionalLightType>("DLT");
        public LiteralEnum<PointLightType> PointLightType = new LiteralEnum<PointLightType>("PLT");
        public LiteralBool IsLightning = new LiteralBool("bLgt");
        public LiteralBool IsMorph = new LiteralBool("bShp");
        public LiteralIntList ModelIdx = new LiteralIntList( "MdNo");

        public AVFXCurve Morph = new AVFXCurve("Moph");
        public AVFXCurve FresnelCurve = new AVFXCurve("FrC");
        public AVFXCurve3Axis FresnelRotation = new AVFXCurve3Axis("FrRt");
        public AVFXCurveColor ColorBegin = new AVFXCurveColor(name: "ColB");
        public AVFXCurveColor ColorEnd = new AVFXCurveColor(name: "ColE");

        List<Base> Attributes;

        public AVFXParticleDataModel() : base("Data")
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

        public override AVFXNode toAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
