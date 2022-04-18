using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Emitter {
    public class AVFXEmitterDataCylinderModel : AVFXGenericData {
        public readonly AVFXEnum<RotationOrder> RotationOrderType = new( "ROT" );
        public readonly AVFXEnum<GenerateMethod> GenerateMethodType = new( "GeMT" );
        public readonly AVFXInt DivideX = new( "DivX" );
        public readonly AVFXInt DivideY = new( "DivY" );
        public readonly AVFXCurve Length = new( "Len" );
        public readonly AVFXCurve Radius = new( "Rad" );
        public readonly AVFXCurve InjectionSpeed = new( "IjS" );
        public readonly AVFXCurve InjectionSpeedRandom = new( "IjSR" );

        public AVFXEmitterDataCylinderModel() : base() {
            Children = new List<AVFXBase> {
                RotationOrderType,
                GenerateMethodType,
                DivideX,
                DivideY,
                Length,
                Radius,
                InjectionSpeed,
                InjectionSpeedRandom
            };

            DivideX.SetValue( 1 );
            DivideY.SetValue( 1 );
        }
    }
}
