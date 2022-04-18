using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Particle {
    public class AVFXParticleDataWindmill : AVFXGenericData {
        public readonly AVFXEnum<WindmillUVType> WindmillUVType = new( "WUvT" );

        public AVFXParticleDataWindmill() : base() {
            Children = new List<AVFXBase> {
                WindmillUVType
            };
        }
    }
}
