using System.Collections.Generic;
using VfxEditor.AVFXLib.Curve;

namespace VfxEditor.AVFXLib.Particle {
    public class AVFXParticleDataDecalRing : AVFXGenericData {
        public readonly AVFXCurve Width = new( "WID" );
        public readonly AVFXFloat ScalingScale = new( "SS" );
        public readonly AVFXFloat RingFan = new( "RF" );

        public AVFXParticleDataDecalRing() : base() {
            Children = new List<AVFXBase> {
                Width,
                ScalingScale,
                RingFan
            };
        }
    }
}
