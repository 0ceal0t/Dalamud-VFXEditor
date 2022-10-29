using System.Collections.Generic;
using VfxEditor.AVFXLib.Curve;

namespace VfxEditor.AVFXLib.Particle {
    public class AVFXParticleDataPolygon : AVFXGenericData {
        public readonly AVFXCurve Count = new( "Cnt" );

        public AVFXParticleDataPolygon() : base() {
            Children = new List<AVFXBase> {
                Count
            };
        }
    }
}
