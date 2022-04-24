using System.Collections.Generic;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Effector {
    public class AVFXEffectorDataPointLight : AVFXGenericData {
        public readonly AVFXCurveColor Color = new();
        public readonly AVFXCurve DistanceScale = new( "DstS" );
        public readonly AVFXCurve3Axis Rotation = new( "Rot" );
        public readonly AVFXCurve3Axis Position = new( "Pos" );
        public readonly AVFXEnum<PointLightAttenuation> PointLightAttenuationType = new( "Attn" );
        public readonly AVFXBool EnableShadow = new( "bSdw" );
        public readonly AVFXBool EnableCharShadow = new( "bChS" );
        public readonly AVFXBool EnableMapShadow = new( "bMpS" );
        public readonly AVFXBool EnableMoveShadow = new( "bMvS" );
        public readonly AVFXFloat ShadowCreateDistanceNear = new( "SCDN" );
        public readonly AVFXFloat ShadowCreateDistanceFar = new( "SCDF" );

        public AVFXEffectorDataPointLight() : base() {
            Children = new List<AVFXBase> {
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
            };
        }
    }
}
