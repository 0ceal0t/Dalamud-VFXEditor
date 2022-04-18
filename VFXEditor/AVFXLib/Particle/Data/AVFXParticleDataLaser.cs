using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
