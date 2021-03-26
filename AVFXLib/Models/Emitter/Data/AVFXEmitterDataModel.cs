using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXEmitterDataModel : AVFXEmitterData
    {
        public LiteralInt ModelIdx = new LiteralInt("MdNo");
        public LiteralEnum<RotationOrder> RotationOrderType = new LiteralEnum<RotationOrder>("ROT");
        public LiteralEnum<GenerateMethod> GenerateMethodType = new LiteralEnum<GenerateMethod>("GeMT");
        public AVFXCurve AX = new AVFXCurve("AnX");
        public AVFXCurve AY = new AVFXCurve("AnY");
        public AVFXCurve AZ = new AVFXCurve("AnZ");
        public AVFXCurve InjectionSpeed = new AVFXCurve("IjS");
        public AVFXCurve InjectionSpeedRandom = new AVFXCurve("IjSR");

        List<Base> Attributes;

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

        public override void read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
        }

        public override void toDefault()
        {
            Assigned = true;
            SetUnAssigned(Attributes);
            ModelIdx.GiveValue(-1);
            SetDefault(RotationOrderType);
            SetDefault(GenerateMethodType);
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
