using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class UiParticleView : UiNodeDropdownView<AvfxParticle> {
        public UiParticleView( AvfxFile file, UiNodeGroup<AvfxParticle> group ) : base( file, group, "Particle", true, true, "default_particle.vfxedit" ) { }

        public override void OnSelect( AvfxParticle item ) { }
    }
}