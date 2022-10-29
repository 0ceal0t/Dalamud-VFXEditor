using System.Collections.Generic;

namespace VfxEditor.AVFXLib.Particle {
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
