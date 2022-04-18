using VFXEditor.AVFXLib.Particle;

namespace VFXEditor.Avfx.Vfx {
    public class UIParticleDataLightModel : UIData {
        public UINodeSelect<UIModel> ModelSelect;
        public UIParameters Parameters;

        public UIParticleDataLightModel( AVFXParticleDataLightModel data, UIParticle particle ) {
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( ModelSelect = new UINodeSelect<UIModel>( particle, "Model", particle.Main.Models, data.ModelIdx ) );
        }

        public override void Dispose() {
            ModelSelect.DeleteSelect();
        }
    }
}
