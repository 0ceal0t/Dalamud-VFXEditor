using System.Collections.Generic;
using VfxEditor.AVFXLib.Curve;

namespace VfxEditor.AVFXLib.Particle {
    public class AVFXParticleDataModel : AVFXGenericData {
        public readonly AVFXInt ModelNumberRandomValue = new( "MNRv" );
        public readonly AVFXEnum<RandomType> ModelNumberRandomType = new( "MNRt" );
        public readonly AVFXInt ModelNumberRandomInterval = new( "MNRi" );
        public readonly AVFXEnum<FresnelType> FresnelType = new( "FrsT" );
        public readonly AVFXEnum<DirectionalLightType> DirectionalLightType = new( "DLT" );
        public readonly AVFXEnum<PointLightType> PointLightType = new( "PLT" );
        public readonly AVFXBool IsLightning = new( "bLgt" );
        public readonly AVFXBool IsMorph = new( "bShp" );
        public AVFXIntList ModelIdx = new( "MdNo" );
        public readonly AVFXCurve AnimationNumber = new( "NoAn" );
        public readonly AVFXCurve Morph = new( "Moph" );
        public readonly AVFXCurve FresnelCurve = new( "FrC" );
        public readonly AVFXCurve3Axis FresnelRotation = new( "FrRt" );
        public readonly AVFXCurveColor ColorBegin = new( name: "ColB" );
        public readonly AVFXCurveColor ColorEnd = new( name: "ColE" );

        public AVFXParticleDataModel() : base() {
            Children = new List<AVFXBase> {
                ModelNumberRandomValue,
                ModelNumberRandomType,
                ModelNumberRandomInterval,
                FresnelType,
                DirectionalLightType,
                PointLightType,
                IsLightning,
                IsMorph,
                ModelIdx,
                AnimationNumber,
                Morph,
                FresnelCurve,
                FresnelRotation,
                ColorBegin,
                ColorEnd
            };
        }
    }
}
