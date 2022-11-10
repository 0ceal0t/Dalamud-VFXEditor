using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticleDataLightModel : UiData {
        public UiNodeSelect<UiModel> ModelSelect;
        public UiParameters Parameters;

        public UiParticleDataLightModel( AVFXParticleDataLightModel data, UiParticle particle ) {
            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( ModelSelect = new UiNodeSelect<UiModel>( particle, "Model", particle.NodeGroups.Models, data.ModelIdx ) );
        }

        public override void Enable() {
            ModelSelect.Enable();
        }

        public override void Disable() {
            ModelSelect.Disable();
        }
    }
}
