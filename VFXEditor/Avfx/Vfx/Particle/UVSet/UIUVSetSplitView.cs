using System;
using System.Collections.Generic;

namespace VFXEditor.Avfx.Vfx {
    public class UIUVSetSplitView : UIItemSplitView<UIParticleUVSet> {
        public UIParticle Particle;

        public UIUVSetSplitView( List<UIParticleUVSet> items, UIParticle particle ) : base( items, true, true ) {
            Particle = particle;
        }

        public override UIParticleUVSet OnNew() {
            var p = Particle.Particle.AddUVSet();
            if( p != null ) {
                return new UIParticleUVSet( p, Particle );
            }
            return null;
        }

        public override void DrawControls( string parentId ) {
            AllowNew = ( Items.Count < 4 );
            base.DrawControls( parentId );
        }

        public override void OnDelete( UIParticleUVSet item ) {
            Particle.Particle.RemoveUVSet( item.UVSet );
        }
    }
}
