using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Emitter {
    public class AVFXEmitterDataSphereModel : AVFXGenericData {
        public readonly AVFXEnum<RotationOrder> RotationOrderType = new( "ROT" );
        public readonly AVFXEnum<GenerateMethod> GenerateMethodType = new( "GeMT" );
        public readonly AVFXInt DivideX = new( "DivX" );
        public readonly AVFXInt DivideY = new( "DivY" );
        public readonly AVFXCurve Radius = new( "Rads" );
        public readonly AVFXCurve InjectionSpeed = new( "IjS" );
        public readonly AVFXCurve InjectionSpeedRandom = new( "IjSR" );

        public AVFXEmitterDataSphereModel() : base() {
            Children = new List<AVFXBase> {
                RotationOrderType,
                GenerateMethodType,
                DivideX,
                DivideY,
                Radius,
                InjectionSpeed,
                InjectionSpeedRandom
            };

            DivideX.SetValue( 1 );
            DivideY.SetValue( 1 );
        }
    }
}
