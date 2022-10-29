using System.Collections.Generic;
using VfxEditor.AVFXLib.Curve;

namespace VfxEditor.AVFXLib.Emitter {
    public class AVFXEmitterDataCone : AVFXGenericData {
        public readonly AVFXEnum<RotationOrder> RotationOrderType = new( "ROT" );
        public readonly AVFXCurve AngleY = new( "AnY" );
        public readonly AVFXCurve OuterSize = new( "OuS" );
        public readonly AVFXCurve InjectionSpeed = new( "IjS" );
        public readonly AVFXCurve InjectionSpeedRandom = new( "IjSR" );
        public readonly AVFXCurve InjectionAngle = new( "IjA" );

        public AVFXEmitterDataCone() : base() {
            Children = new List<AVFXBase> {
                RotationOrderType,
                AngleY,
                OuterSize,
                InjectionSpeed,
                InjectionSpeedRandom,
                InjectionAngle
            };
        }
    }
}
