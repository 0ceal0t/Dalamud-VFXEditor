using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleDataModel : AvfxData {
        public readonly AvfxInt ModelNumberRandomValue = new( "Model Number Random", "MNRv" );
        public readonly AvfxEnum<RandomType> ModelNumberRandomType = new( "Model Number Random Type", "MNRt" );
        public readonly AvfxInt ModelNumberRandomInterval = new( "Model Number Random Interval", "MNRi" );
        public readonly AvfxEnum<FresnelType> FresnelType = new( "Fresnel Type", "FrsT" );
        public readonly AvfxEnum<DirectionalLightType> DirectionalLightType = new( "Directional Light Type", "DLT" );
        public readonly AvfxEnum<PointLightType> PointLightType = new( "Point Light Type", "PLT" );
        public readonly AvfxBool IsLightning = new( "Is Lightning", "bLgt" );
        public readonly AvfxBool IsMorph = new( "Is Morph", "bShp" );
        public AvfxIntList ModelIdx = new( "Model Index", "MdNo" );
        public readonly AvfxCurve AnimationNumber = new( "Animation Number", "NoAn" );
        public readonly AvfxCurve Morph = new( "Morph", "Moph" );
        public readonly AvfxCurve FresnelCurve = new( "Fresnel Curve", "FrC" );
        public readonly AvfxCurve3Axis FresnelRotation = new( "Fresnel Rotation", "FrRt" );
        public readonly AvfxCurveColor ColorBegin = new( name: "Color Begin", "ColB" );
        public readonly AvfxCurveColor ColorEnd = new( name: "Color End", "ColE" );

        public readonly UiNodeSelectList<AvfxModel> ModelSelect;
        public readonly UiParameters Parameters;

        public AvfxParticleDataModel( AvfxParticle particle ) : base() {
            Children = new() {
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

            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( ModelSelect = new UiNodeSelectList<AvfxModel>( particle, "Model", particle.NodeGroups.Models, ModelIdx ) );
            Parameters.Add( ModelNumberRandomValue );
            Parameters.Add( ModelNumberRandomType );
            Parameters.Add( ModelNumberRandomInterval );
            Parameters.Add( FresnelType );
            Parameters.Add( DirectionalLightType );
            Parameters.Add( PointLightType );
            Parameters.Add( IsLightning );
            Parameters.Add( IsMorph );

            Tabs.Add( Morph );
            Tabs.Add( FresnelCurve );
            Tabs.Add( FresnelRotation );
            Tabs.Add( ColorBegin );
            Tabs.Add( ColorEnd );
            Tabs.Add( AnimationNumber );
        }

        public override void Enable() {
            ModelSelect.Enable();
        }

        public override void Disable() {
            ModelSelect.Disable();
        }
    }
}
