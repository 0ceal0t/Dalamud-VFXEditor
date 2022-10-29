using System.Collections.Generic;

namespace VfxEditor.AVFXLib.Particle {
    public class AVFXParticleDataDecal : AVFXGenericData {
        public readonly AVFXFloat ScalingScale = new( "SS" );

        public AVFXParticleDataDecal() : base() {
            Children = new List<AVFXBase> {
                ScalingScale
            };
        }
    }
}
