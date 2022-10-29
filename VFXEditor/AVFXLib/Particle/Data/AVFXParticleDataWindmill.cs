using System.Collections.Generic;

namespace VfxEditor.AVFXLib.Particle {
    public class AVFXParticleDataWindmill : AVFXGenericData {
        public readonly AVFXEnum<WindmillUVType> WindmillUVType = new( "WUvT" );

        public AVFXParticleDataWindmill() : base() {
            Children = new List<AVFXBase> {
                WindmillUVType
            };
        }
    }
}
