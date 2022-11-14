using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class UiParticleView : UiNodeDropdownView<AvfxParticle> {
        public UiParticleView( AvfxFile file, UiNodeGroup<AvfxParticle> group ) : base( file, group, "Particle", true, true, "default_particle.vfxedit" ) { }

        public override void OnSelect( AvfxParticle item ) { }

        public override AvfxParticle Read( BinaryReader reader, int size, bool hasDependencies ) {
            var item = new AvfxParticle( File.NodeGroupSet, hasDependencies );
            item.Read( reader, size );
            return item;
        }
    }
}