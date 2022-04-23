using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Particle;

namespace VFXEditor.AVFX.VFX {
    public class UIParticleDataModel : UIData {
        public UINodeSelectList<UIModel> ModelSelect;
        public UIParameters Parameters;

        public UIParticleDataModel( AVFXParticleDataModel data, UIParticle particle ) {
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( ModelSelect = new UINodeSelectList<UIModel>( particle, "Model", particle.Main.Models, data.ModelIdx ) );
            Parameters.Add( new UIInt( "Model Number Random", data.ModelNumberRandomValue ) );
            Parameters.Add( new UICombo<RandomType>( "Model Number Random Type", data.ModelNumberRandomType ) );
            Parameters.Add( new UIInt( "Model Number Random Interval", data.ModelNumberRandomInterval ) );
            Parameters.Add( new UICombo<FresnelType>( "Fresnel Type", data.FresnelType ) );
            Parameters.Add( new UICombo<DirectionalLightType>( "Directional Light Type", data.DirectionalLightType ) );
            Parameters.Add( new UICombo<PointLightType>( "Point Light Type", data.PointLightType ) );
            Parameters.Add( new UICheckbox( "Is Lightning", data.IsLightning ) );
            Parameters.Add( new UICheckbox( "Is Morph", data.IsMorph ) );

            Tabs.Add( new UICurve( data.Morph, "Morph" ) );
            Tabs.Add( new UICurve( data.FresnelCurve, "Fresnel Curve" ) );
            Tabs.Add( new UICurve3Axis( data.FresnelRotation, "Fresnel Rotation" ) );
            Tabs.Add( new UICurveColor( data.ColorBegin, "Color Begin" ) );
            Tabs.Add( new UICurveColor( data.ColorEnd, "Color End" ) );
            Tabs.Add( new UICurve( data.AnimationNumber, "Animation Number" ) );
        }

        public override void Dispose() {
            ModelSelect.DeleteSelect();
        }
    }
}
