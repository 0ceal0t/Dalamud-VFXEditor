using System.Collections.Generic;
using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiUvSetSplitView : UiItemSplitView<UiParticleUvSet> {
        public UiParticle Particle;

        public UiUvSetSplitView( List<UiParticleUvSet> items, UiParticle particle ) : base( items ) {
            Particle = particle;
        }

        public override UiParticleUvSet OnNew() {
            //var p = Particle.Particle.AddUvSet();
            //if( p != null )  return new UiParticleUvSet( p, Particle );
            return null;
        }

        public override void DrawControls( string parentId ) {
            AllowNew = ( Items.Count < 4 );
            base.DrawControls( parentId );
        }

        public override void RemoveFromAvfx( UiParticleUvSet item ) {
            Particle.Particle.UvSets.Remove( item.UvSet );
        }

        public override void AddToAvfx( UiParticleUvSet item, int idx ) {
            Particle.Particle.UvSets.Insert( idx, item.UvSet );
        }
    }
}
