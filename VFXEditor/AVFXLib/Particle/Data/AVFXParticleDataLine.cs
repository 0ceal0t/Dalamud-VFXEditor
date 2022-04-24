using System.Collections.Generic;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Particle {
    public class AVFXParticleDataLine : AVFXGenericData {
        public readonly AVFXInt LineCount = new( "LnCT" );
        public readonly AVFXCurve Length = new( "Len" );
        public readonly AVFXCurve LengthRandom = new( "LenR" );
        public readonly AVFXCurveColor ColorBegin = new( name: "ColB" );
        public readonly AVFXCurveColor ColorEnd = new( name: "ColE" );

        public AVFXParticleDataLine() : base() {
            Children = new List<AVFXBase> {
                LineCount,
                Length,
                LengthRandom,
                ColorBegin,
                ColorEnd
            };
        }
    }
}
