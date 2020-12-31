using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXEffectorDataCameraQuake : AVFXEffectorData
    {
        public AVFXCurve Attenuation = new AVFXCurve("attenuation", "Att");
        public AVFXCurve RadiusOut = new AVFXCurve("radiusOuter", "RdO");
        public AVFXCurve RadiusIn = new AVFXCurve("radiusInner", "RdI");
        public AVFXCurve3Axis Rotation = new AVFXCurve3Axis("rotation", "Rot");
        public AVFXCurve3Axis Position = new AVFXCurve3Axis("position", "Pos");

        List<Base> Attributes;

        public AVFXEffectorDataCameraQuake(string jsonPath) : base(jsonPath, "Data")
        {
            Attributes = new List<Base>(new Base[]{
                Attenuation,
                RadiusOut,
                RadiusIn,
                Rotation,
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
            SetUnAssigned(Attributes);
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
