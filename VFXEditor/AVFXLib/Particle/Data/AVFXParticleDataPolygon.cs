using System.Collections.Generic;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Particle {
    public class AVFXParticleDataPolygon : AVFXGenericData {
        public readonly AVFXCurve Count = new( "Cnt" );

        public AVFXParticleDataPolygon() : base() {
            Children = new List<AVFXBase> {
                Count
            };
        }
    }
}
