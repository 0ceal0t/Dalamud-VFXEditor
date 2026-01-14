using System.IO;

namespace VfxEditor.AvfxFormat {
    public class UiParticleView : AvfxNodeDropdownView<AvfxParticle> {
        public UiParticleView( AvfxFile file, NodeGroup<AvfxParticle> group ) : base( file, group, "Particle", true, true, "default_particle.vfxedit" ) { }

        public override void OnSelect( AvfxParticle item ) { }

        public override AvfxParticle Read( BinaryReader reader, int size ) {
            var item = new AvfxParticle( File, File.NodeGroupSet );
            item.Read( reader, size );
            return item;
        }
    }
}