using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Emitter {
    public class AVFXEmitterDataModel : AVFXGenericData {
        public readonly AVFXInt ModelIdx = new( "MdNo" );
        public readonly AVFXEnum<RotationOrder> RotationOrderType = new( "ROT" );
        public readonly AVFXEnum<GenerateMethod> GenerateMethodType = new( "GeMT" );
        public readonly AVFXCurve AX = new( "AnX" );
        public readonly AVFXCurve AY = new( "AnY" );
        public readonly AVFXCurve AZ = new( "AnZ" );
        public readonly AVFXCurve InjectionSpeed = new( "IjS" );
        public readonly AVFXCurve InjectionSpeedRandom = new( "IjSR" );

        public AVFXEmitterDataModel() : base() {
            Children = new List<AVFXBase> {
                ModelIdx,
                RotationOrderType,
                GenerateMethodType,
                AX,
                AY,
                AZ,
                InjectionSpeed,
                InjectionSpeedRandom
            };

            ModelIdx.SetValue( -1 );
        }
    }
}
