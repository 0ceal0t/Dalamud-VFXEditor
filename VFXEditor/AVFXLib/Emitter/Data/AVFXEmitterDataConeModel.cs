using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Emitter {
    public class AVFXEmitterDataConeModel : AVFXGenericData {
        public readonly AVFXEnum<RotationOrder> RotationOrderType = new( "ROT" );
        public readonly AVFXEnum<GenerateMethod> GenerateMethodType = new( "GeMT" );
        public readonly AVFXInt DivideX = new( "DivX" );
        public readonly AVFXInt DivideY = new( "DivY" );
        public readonly AVFXCurve AX = new( "AnX" );
        public readonly AVFXCurve AY = new( "AnY" );
        public readonly AVFXCurve Radius = new( "Rad" );
        public readonly AVFXCurve InjectionSpeed = new( "IjS" );
        public readonly AVFXCurve InjectionSpeedRandom = new( "IjSR" );
        public readonly AVFXCurve InjectionAngle = new( "IjA" );

        public AVFXEmitterDataConeModel() : base() {
            Children = new List<AVFXBase> {
                RotationOrderType,
                GenerateMethodType,
                DivideX,
                DivideY,
                AX,
                AY,
                Radius,
                InjectionSpeed,
                InjectionSpeedRandom,
                InjectionAngle
            };
        }
    }
}
