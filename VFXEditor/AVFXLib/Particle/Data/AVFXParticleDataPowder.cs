using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Particle {
    public class AVFXParticleDataPowder : AVFXGenericData {
        public readonly AVFXBool IsLightning = new( "bLgt" );
        public readonly AVFXEnum<DirectionalLightType> DirectionalLightType = new( "LgtT" );
        public readonly AVFXFloat CenterOffset = new( "CnOf" );

        public AVFXParticleDataPowder() : base() {
            Children = new List<AVFXBase> {
                IsLightning,
                DirectionalLightType,
                CenterOffset
            };
        }
    }
}
