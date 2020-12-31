using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXEmitterDataCylinderModel : AVFXEmitterData
    {
        public LiteralEnum<RotationOrder> RotationOrderType = new LiteralEnum<RotationOrder>("rotationOrder", "ROT");
        public LiteralEnum<GenerateMethod> GenerateMethodType = new LiteralEnum<GenerateMethod>("generateMethod", "GeMT");
        public LiteralInt DivideX = new LiteralInt("divideX", "DivX");
        public LiteralInt DivideY = new LiteralInt("divideY", "DivY");

        public AVFXCurve Length = new AVFXCurve("length", "Len");
        public AVFXCurve Radius = new AVFXCurve("radius", "Rad");
        public AVFXCurve InjectionSpeed = new AVFXCurve("injectionSpeed", "IjS");
        public AVFXCurve InjectionSpeedRandom = new AVFXCurve("injectionSpeedRandom", "IjSR");

        List<Base> Attributes;

        public AVFXEmitterDataCylinderModel(string jsonPath) : base(jsonPath, "Data")
        {
            Attributes = new List<Base>(new Base[] {
                RotationOrderType,
                GenerateMethodType,
                DivideX,
                DivideY,
                Length,
                Radius,
                InjectionSpeed,
                InjectionSpeedRandom
            });
        }

        public override void toDefault()
        {
            Assigned = true;
            SetUnAssigned(Attributes);
            SetDefault(RotationOrderType);
            SetDefault(GenerateMethodType);
            DivideX.GiveValue(1);
            DivideY.GiveValue(1);
        }

        public override void read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
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
