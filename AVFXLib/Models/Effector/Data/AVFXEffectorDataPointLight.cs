using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXEffectorDataPointLight : AVFXEffectorData
    {
        public AVFXCurveColor Color = new AVFXCurveColor("color");
        public AVFXCurve DistanceScale = new AVFXCurve("distanceScale", "DstS");
        public AVFXCurve3Axis Rotation = new AVFXCurve3Axis("rotation", "Rot");
        public AVFXCurve3Axis Position = new AVFXCurve3Axis("position", "Pos");
        public LiteralEnum<PointLightAttenuation> PointLightAttenuationType = new LiteralEnum<PointLightAttenuation>("pointLightAttenuationType", "Attn");
        public LiteralBool EnableShadow = new LiteralBool("enableShadow", "bSdw");
        public LiteralBool EnableCharShadow = new LiteralBool("enableCharShadow", "bChS");
        public LiteralBool EnableMapShadow = new LiteralBool("enableMapShadow", "bMpS");
        public LiteralBool EnableMoveShadow = new LiteralBool("enableMoveShadow", "bMvS");
        public LiteralFloat ShadowCreateDistanceNear = new LiteralFloat("shadowCreateDistanceNear", "SCDN");
        public LiteralFloat ShadowCreateDistanceFar = new LiteralFloat("shadowCreateDistanceFar", "SCDF");

        List<Base> Attributes;

        public AVFXEffectorDataPointLight(string jsonPath) : base(jsonPath, "Data")
        {
            Attributes = new List<Base>(new Base[]{
                Color,
                DistanceScale,
                Rotation,
                Position,
                PointLightAttenuationType,
                EnableShadow,
                EnableCharShadow,
                EnableMapShadow,
                EnableMoveShadow,
                ShadowCreateDistanceNear,
                ShadowCreateDistanceFar
            });
        }

        public override void toDefault()
        {
            Assigned = true;
            SetDefault(Attributes);
            SetUnAssigned(Color);
            SetUnAssigned(DistanceScale);
            SetUnAssigned(Rotation);
            SetUnAssigned(Position);
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
