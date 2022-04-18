using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Particle {
    public class AVFXParticleDataParameter : AVFXGenericData {
        public AVFXParticleDataParameter() : base() {
            Children = new List<AVFXBase> {
            };
        }
    }
}
