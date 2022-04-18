using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Effector {
    public class AVFXEffectorDataCameraQuake : AVFXGenericData {
        public readonly AVFXCurve Attenuation = new( "Att" );
        public readonly AVFXCurve RadiusOut = new( "RdO" );
        public readonly AVFXCurve RadiusIn = new( "RdI" );
        public readonly AVFXCurve3Axis Rotation = new( "Rot" );
        public readonly AVFXCurve3Axis Position = new( "Pos" );

        public AVFXEffectorDataCameraQuake() : base() {
            Children = new List<AVFXBase> {
                Attenuation,
                RadiusOut,
                RadiusIn,
                Rotation,
                Position
            };
        }
    }
}
