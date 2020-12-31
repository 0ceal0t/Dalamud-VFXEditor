using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXBinderProperty : Base
    {
        public string Name = "PrpS";

        public LiteralEnum<BindPoint> BindPointType = new LiteralEnum<BindPoint>("bindPointType", "BPT");
        public LiteralEnum<BindTargetPoint> BindTargetPointType = new LiteralEnum<BindTargetPoint>("bindTargetPointType", "BPTP");
        public LiteralString BinderName = new LiteralString("name", "Name", fixedSize:8);
        public LiteralInt BindPointId = new LiteralInt("bindPointId", "BPID");
        public LiteralInt GenerateDelay = new LiteralInt("generateDelay", "GenD");
        public LiteralInt CoordUpdateFrame = new LiteralInt("coordUpdateFrame", "CoUF");
        public LiteralBool RingEnable = new LiteralBool("ringEnable", "bRng");
        public LiteralInt RingProgressTime = new LiteralInt("ringProgressTime", "RnPT");
        public LiteralFloat RingPositionX = new LiteralFloat("ringPositionX", "RnPX");
        public LiteralFloat RingPositionY = new LiteralFloat("ringPositionY", "RnPY");
        public LiteralFloat RingPositionZ = new LiteralFloat("ringPositionZ", "RnPZ");
        public LiteralFloat RingRadius = new LiteralFloat("ringRadius", "RnRd");
        public AVFXCurve3Axis Position = new AVFXCurve3Axis("position", "Pos");

        List<Base> Attributes;

        public AVFXBinderProperty(string jsonPath, string name) : base(jsonPath, name)
        {
            Name = name;
            Attributes = new List<Base>(new Base[]{
                BindPointType,
                BindTargetPointType,
                BinderName,
                BindPointId,
                GenerateDelay,
                CoordUpdateFrame,
                RingEnable,
                RingProgressTime,
                RingPositionX,
                RingPositionY,
                RingPositionZ,
                RingRadius,
                Position
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
            SetUnAssigned(Position);
            BinderName.GiveValue("null");
        }

        public override JToken toJSON()
        {
            JObject elem = new JObject();
            PutJSON(elem, Attributes);
            return elem;
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode(Name);
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
