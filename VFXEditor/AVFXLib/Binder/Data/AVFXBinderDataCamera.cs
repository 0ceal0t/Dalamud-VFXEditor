using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Binder {
    public class AVFXBinderDataCamera : AVFXGenericData {
        public readonly AVFXCurve Distance = new( "Dst" );
        public readonly AVFXCurve DistanceRandom = new( "DstR" );

        public AVFXBinderDataCamera() : base() {
            Children = new List<AVFXBase> {
                Distance,
                DistanceRandom
            };
        }
    }
}
