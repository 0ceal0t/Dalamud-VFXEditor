using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXEffectorDataPointLight : AVFXEffectorData
    {
        public AVFXCurveColor Color = new AVFXCurveColor();
        public AVFXCurve DistanceScale = new AVFXCurve("DstS");
        public AVFXCurve3Axis Rotation = new AVFXCurve3Axis("Rot");
        public AVFXCurve3Axis Position = new AVFXCurve3Axis("Pos");
        public LiteralEnum<PointLightAttenuation> PointLightAttenuationType = new LiteralEnum<PointLightAttenuation>("Attn");
        public LiteralBool EnableShadow = new LiteralBool("bSdw");
        public LiteralBool EnableCharShadow = new LiteralBool("bChS");
        public LiteralBool EnableMapShadow = new LiteralBool("bMpS");
        public LiteralBool EnableMoveShadow = new LiteralBool("bMvS");
        public LiteralFloat ShadowCreateDistanceNear = new LiteralFloat("SCDN");
        public LiteralFloat ShadowCreateDistanceFar = new LiteralFloat("SCDF");

        List<Base> Attributes;

        public AVFXEffectorDataPointLight() : base("Data")
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

        public override void ToDefault()
        {
            Assigned = true;
            SetDefault(Attributes);
            SetUnAssigned(Color);
            SetUnAssigned(DistanceScale);
            SetUnAssigned(Rotation);
            SetUnAssigned(Position);
        }

        public override void Read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
        }

        public override AVFXNode ToAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
