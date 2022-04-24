using System.Collections.Generic;

namespace VFXEditor.AVFXLib.Particle {
    public class AVFXParticleDataLightModel : AVFXGenericData {
        public readonly AVFXInt ModelIdx = new( "MNO", size: 1 );

        public AVFXParticleDataLightModel() : base() {
            Children = new List<AVFXBase> {
                ModelIdx
            };
        }
    }
}
