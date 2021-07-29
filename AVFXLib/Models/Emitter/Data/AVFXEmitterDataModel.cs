using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXEmitterDataModel : AVFXEmitterData
    {
        public LiteralInt ModelIdx = new("MdNo");
        public LiteralEnum<RotationOrder> RotationOrderType = new("ROT");
        public LiteralEnum<GenerateMethod> GenerateMethodType = new("GeMT");
        public AVFXCurve AX = new("AnX");
        public AVFXCurve AY = new("AnY");
        public AVFXCurve AZ = new("AnZ");
        public AVFXCurve InjectionSpeed = new("IjS");
        public AVFXCurve InjectionSpeedRandom = new("IjSR");
        readonly List<Base> Attributes;

        public AVFXEmitterDataModel() : base("Data")
        {
            Attributes = new List<Base>(new Base[] {
                ModelIdx,
                RotationOrderType,
                GenerateMethodType,
                AX,
                AY,
                AZ,
                InjectionSpeed,
                InjectionSpeedRandom
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
            SetUnAssigned(Attributes);
            ModelIdx.GiveValue(-1);
            SetDefault(RotationOrderType);
            SetDefault(GenerateMethodType);
        }

        public override AVFXNode ToAVFX()
        {
            var dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
