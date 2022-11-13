using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleDataLightModel : AvfxData {
        public readonly AvfxInt ModelIdx = new( "Model Index", "MNO", size: 1 );

        public readonly UiNodeSelect<AvfxModel> ModelSelect;
        public readonly UiParameters Display;

        public AvfxParticleDataLightModel( AvfxParticle particle ) : base() {
            Parsed = new() {
                ModelIdx
            };

            DisplayTabs.Add( Display = new UiParameters( "Parameters" ) );
            Display.Add( ModelSelect = new UiNodeSelect<AvfxModel>( particle, "Model", particle.NodeGroups.Models, ModelIdx ) );
        }

        public override void Enable() {
            ModelSelect.Enable();
        }

        public override void Disable() {
            ModelSelect.Disable();
        }
    }
}
