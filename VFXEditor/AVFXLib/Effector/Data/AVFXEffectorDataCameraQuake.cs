using System.Collections.Generic;
using VfxEditor.AVFXLib.Curve;

namespace VfxEditor.AVFXLib.Effector {
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
