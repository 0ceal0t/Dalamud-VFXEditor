namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataLightModel : AvfxDataWithParameters {
        public readonly AvfxInt ModelIdx = new( "Model Index", "MNO", size: 1 );
        public readonly AvfxNodeSelect<AvfxModel> ModelSelect;

        public AvfxParticleDataLightModel( AvfxParticle particle ) : base() {
            Parsed = [
                ModelIdx
            ];

            ParameterTab.Add( ModelSelect = new AvfxNodeSelect<AvfxModel>( particle, "Model", particle.NodeGroups.Models, ModelIdx ) );
        }

        public override void Enable() => ModelSelect.Enable();

        public override void Disable() => ModelSelect.Disable();
    }
}
