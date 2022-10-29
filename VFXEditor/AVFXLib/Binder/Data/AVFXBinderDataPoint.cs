using System.Collections.Generic;
using VfxEditor.AVFXLib.Curve;

namespace VfxEditor.AVFXLib.Binder {
    public class AVFXBinderDataPoint : AVFXGenericData {
        public readonly AVFXCurve SpringStrength = new( "SpS" );

        public AVFXBinderDataPoint() : base() {
            Children = new List<AVFXBase> {
                SpringStrength
            };
        }
    }
}
