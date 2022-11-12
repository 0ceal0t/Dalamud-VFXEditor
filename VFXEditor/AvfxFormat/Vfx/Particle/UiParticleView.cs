using System.IO;
using System.Linq;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticleView : UiNodeDropdownView<UiParticle> {
        public UiParticleView( AvfxFile vfxFile, AVFXMain avfx, UiNodeGroup<UiParticle> group ) : base( vfxFile, avfx, group, "Particle", true, true, "default_particle.vfxedit" ) { }

        public override void RemoveFromAvfx( UiParticle item ) => Avfx.Particles.Remove( item.Particle );

        public override void AddToAvfx( UiParticle item, int idx ) => Avfx.Particles.Insert( idx, item.Particle );

        public override void OnExport( BinaryWriter writer, UiParticle item ) => item.Write( writer );

        public override UiParticle AddToAvfx( BinaryReader reader, int size, bool hasDepdencies ) {
            var item = new AVFXParticle();
            item.Read( reader, size );
            Avfx.Particles.Add( item );
            return new UiParticle( item, VfxFile.NodeGroupSet, hasDepdencies );
        }

        public override void OnSelect( UiParticle item ) { }
    }
}
