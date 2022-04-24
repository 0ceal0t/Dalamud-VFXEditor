using System.Collections.Generic;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Particle {
    public class AVFXParticleDataLaser : AVFXGenericData {
        public readonly AVFXCurve Length = new( "Len" );
        public readonly AVFXCurve Width = new( "Wdt" );

        public AVFXParticleDataLaser() : base() {
            Children = new List<AVFXBase> {
                Length,
                Width
            };
        }
    }
}
