using System.Collections.Generic;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Binder {
    public class AVFXBinderDataPoint : AVFXGenericData {
        public readonly AVFXCurve SpringStrength = new( "SpS" );

        public AVFXBinderDataPoint() : base() {
            Children = new List<AVFXBase> {
                SpringStrength
            };
        }
    }
}
