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
        public LiteralInt ModelIdx = new LiteralInt("modelIdx", "MdNo");
        public LiteralEnum<RotationOrder> RotationOrderType = new LiteralEnum<RotationOrder>("rotationOrder", "ROT");
        public LiteralEnum<GenerateMethod> GenerateMethodType = new LiteralEnum<GenerateMethod>("generateMethod", "GeMT");
        public AVFXCurve AX = new AVFXCurve("angleX", "AX");
        public AVFXCurve AY = new AVFXCurve("angleY", "AY");
        public AVFXCurve AZ = new AVFXCurve("angleZ", "AZ");
        public AVFXCurve InjectionSpeed = new AVFXCurve("injectionSpeed", "IjS");
        public AVFXCurve InjectionSpeedRandom = new AVFXCurve("injectionSpeedRandom", "IjSR");

        List<Base> Attributes;

        public AVFXEmitterDataModel(string jsonPath) : base(jsonPath, "Data")
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
