using VfxEditor.Formats.AvfxFormat.Base;
using VfxEditor.Formats.AvfxFormat.Components.Int;
using VfxEditor.Formats.AvfxFormat.Model;
using VfxEditor.Formats.AvfxFormat.Nodes.Select;

namespace VfxEditor.Formats.AvfxFormat.Particle.Data {
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
