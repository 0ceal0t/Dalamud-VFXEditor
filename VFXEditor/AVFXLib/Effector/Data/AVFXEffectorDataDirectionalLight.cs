using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
