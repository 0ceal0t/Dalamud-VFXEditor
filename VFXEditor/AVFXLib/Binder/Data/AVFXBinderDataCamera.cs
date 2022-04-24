using System.Collections.Generic;
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
