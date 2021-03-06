using AVFXLib.Models;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX {
    public class UIUVSetSplitView : UIItemSplitView<UIParticleUVSet> {
        public UIParticle Particle;

        public UIUVSetSplitView( List<UIParticleUVSet> items, UIParticle particle ) : base( items, true, true ) {
            Particle = particle;
        }

        public override UIParticleUVSet OnNew() {
            var p = Particle.Particle.addUvSet();
            if( p != null ) {
                return new UIParticleUVSet( p, Particle );
            }
            return null;
        }

        public override void DrawNewButton( string parentId ) {
            if(Items.Count < 4 ) {
                base.DrawNewButton( parentId );
            }
        }

        public override void OnDelete( UIParticleUVSet item ) {
            Particle.Particle.removeUvSet( item.UVSet );
        }
    }
}
