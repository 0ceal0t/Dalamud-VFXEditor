using System.Collections.Generic;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Effector {
    public class AVFXEffectorDataDirectionalLight : AVFXGenericData {
        public readonly AVFXCurveColor Ambient = new( "Amb" );
        public readonly AVFXCurveColor Color = new();
        public readonly AVFXCurve Power = new( "Pow" );
        public readonly AVFXCurve3Axis Rotation = new( "Rot" );

        public AVFXEffectorDataDirectionalLight() : base() {
            Children = new List<AVFXBase> {
                Ambient,
                Color,
                Power,
                Rotation
            };
        }
    }
}
