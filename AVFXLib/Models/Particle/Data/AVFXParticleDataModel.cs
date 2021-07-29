using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXParticleDataModel : AVFXParticleData
    {
        public LiteralInt ModelNumberRandomValue = new("MNRv");
        public LiteralEnum<RandomType> ModelNumberRandomType = new("MNRt");
        public LiteralInt ModelNumberRandomInterval = new("MNRi");
        public LiteralEnum<FresnelType> FresnelType = new("FrsT");
        public LiteralEnum<DirectionalLightType> DirectionalLightType = new("DLT");
        public LiteralEnum<PointLightType> PointLightType = new("PLT");
        public LiteralBool IsLightning = new("bLgt");
        public LiteralBool IsMorph = new("bShp");
        public LiteralIntList ModelIdx = new( "MdNo");

        public AVFXCurve Morph = new("Moph");
        public AVFXCurve FresnelCurve = new("FrC");
        public AVFXCurve3Axis FresnelRotation = new("FrRt");
        public AVFXCurveColor ColorBegin = new(name: "ColB");
        public AVFXCurveColor ColorEnd = new(name: "ColE");
        readonly List<Base> Attributes;

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

        public override void Read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
        }

        public override void ToDefault()
        {
            Assigned = true;
            SetDefault(Attributes);
            SetUnAssigned(Morph);
            SetUnAssigned(FresnelCurve);
            SetUnAssigned(FresnelRotation);
            SetUnAssigned(ColorBegin);
            SetUnAssigned(ColorEnd);
        }

        public override AVFXNode ToAVFX()
        {
            var dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
