using System.Collections.Generic;
using VfxEditor.AVFXLib.Curve;

namespace VfxEditor.AVFXLib.Particle {
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
