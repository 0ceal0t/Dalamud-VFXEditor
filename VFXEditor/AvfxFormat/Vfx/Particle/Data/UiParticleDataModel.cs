using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticleDataModel : UiData {
        public UiNodeSelectList<UiModel> ModelSelect;
        public UiParameters Parameters;

        public UiParticleDataModel( AVFXParticleDataModel data, UiParticle particle ) {
            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( ModelSelect = new UiNodeSelectList<UiModel>( particle, "Model", particle.NodeGroups.Models, data.ModelIdx ) );
            Parameters.Add( new UiInt( "Model Number Random", data.ModelNumberRandomValue ) );
            Parameters.Add( new UiCombo<RandomType>( "Model Number Random Type", data.ModelNumberRandomType ) );
            Parameters.Add( new UiInt( "Model Number Random Interval", data.ModelNumberRandomInterval ) );
            Parameters.Add( new UiCombo<FresnelType>( "Fresnel Type", data.FresnelType ) );
            Parameters.Add( new UiCombo<DirectionalLightType>( "Directional Light Type", data.DirectionalLightType ) );
            Parameters.Add( new UiCombo<PointLightType>( "Point Light Type", data.PointLightType ) );
            Parameters.Add( new UiCheckbox( "Is Lightning", data.IsLightning ) );
            Parameters.Add( new UiCheckbox( "Is Morph", data.IsMorph ) );

            Tabs.Add( new UiCurve( data.Morph, "Morph" ) );
            Tabs.Add( new UiCurve( data.FresnelCurve, "Fresnel Curve" ) );
            Tabs.Add( new UiCurve3Axis( data.FresnelRotation, "Fresnel Rotation" ) );
            Tabs.Add( new UiCurveColor( data.ColorBegin, "Color Begin" ) );
            Tabs.Add( new UiCurveColor( data.ColorEnd, "Color End" ) );
            Tabs.Add( new UiCurve( data.AnimationNumber, "Animation Number" ) );
        }

        public override void Disable() {
            ModelSelect.Disable();
        }
    }
}
