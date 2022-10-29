using System.Collections.Generic;
using VfxEditor.AVFXLib.Curve;

namespace VfxEditor.AVFXLib.Binder {
    public class AVFXBinderDataSpline : AVFXGenericData {
        public readonly AVFXCurve CarryOverFactor = new( "COF" );
        public readonly AVFXCurve CarryOverFactorRandom = new( "COFR" );

        public AVFXBinderDataSpline() : base() {
            Children = new List<AVFXBase> {
                CarryOverFactor,
                CarryOverFactorRandom
            };
        }
    }
}
