namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataLightModel : AvfxData {
        public readonly AvfxInt ModelIdx = new( "Model Index", "MNO", size: 1 );

        public readonly UiNodeSelect<AvfxModel> ModelSelect;
        public readonly UiDisplayList Display;

        public AvfxParticleDataLightModel( AvfxParticle particle ) : base() {
            Parsed = [
                ModelIdx
            ];

            DisplayTabs.Add( Display = new UiDisplayList( "Parameters" ) );
            Display.Add( ModelSelect = new UiNodeSelect<AvfxModel>( particle, "Model", particle.NodeGroups.Models, ModelIdx ) );
        }

        public override void Enable() => ModelSelect.Enable();

        public override void Disable() => ModelSelect.Disable();
    }
}
