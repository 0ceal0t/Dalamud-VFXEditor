using System.IO;
using System.Linq;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticleView : UiNodeDropdownView<UiParticle> {
        public UiParticleView( AvfxFile vfxFile, AVFXMain avfx, UiNodeGroup<UiParticle> group ) : base( vfxFile, avfx, group, "Particle", true, true, "default_particle.vfxedit" ) { }

        public override void OnDelete( UiParticle item ) => AVFX.RemoveParticle( item.Particle );

        public override void OnExport( BinaryWriter writer, UiParticle item ) => item.Write( writer );

        public override UiParticle OnImport( BinaryReader reader, int size, bool has_dependencies = false ) {
            var item = new AVFXParticle();
            item.Read( reader, size );
            AVFX.AddParticle( item );
            return new UiParticle( item, VfxFile.NodeGroupSet, has_dependencies );
        }

        public override void OnSelect( UiParticle item ) { }
    }
}
