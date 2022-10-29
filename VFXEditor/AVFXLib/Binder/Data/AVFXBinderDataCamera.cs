using System.Collections.Generic;
using VfxEditor.AVFXLib.Curve;

namespace VfxEditor.AVFXLib.Binder {
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
