using System.Collections.Generic;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiUvSetSplitView : UiItemSplitView<UiParticleUvSet> {
        public UiParticle Particle;

        public UiUvSetSplitView( List<UiParticleUvSet> items, UiParticle particle ) : base( items, true, true ) {
            Particle = particle;
        }

        public override UiParticleUvSet OnNew() {
            var p = Particle.Particle.AddUvSet();
            if( p != null ) {
                return new UiParticleUvSet( p, Particle );
            }
            return null;
        }

        public override void DrawControls( string parentId ) {
            AllowNew = ( Items.Count < 4 );
            base.DrawControls( parentId );
        }

        public override void OnDelete( UiParticleUvSet item ) {
            Particle.Particle.RemoveUvSet( item.UvSet );
        }
    }
}
