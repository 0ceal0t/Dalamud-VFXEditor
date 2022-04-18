using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.AVFXLib.Curve;

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
