using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using AVFXLib.AVFX;

namespace VFXEditor.UI.VFX
{
    public class UIParticleView : UIDropdownView<UIParticle>
    {
        public UIParticleView( AVFXBase avfx ) : base( avfx, "##PTCL", "Select a Particle" )
        {
            Group = UINode._Particles;
            Group.Items = AVFX.Particles.Select( item => new UIParticle( item, this ) ).ToList();
        }

        public override UIParticle OnNew() {
            return new UIParticle( AVFX.addParticle(), this );
        }
        public override void OnDelete( UIParticle item ) {
            AVFX.removeParticle( item.Particle );
        }
        public override byte[] OnExport( UIParticle item ) {
            return item.Particle.toAVFX().toBytes();
        }
        public override UIParticle OnImport( AVFXNode node ) {
            AVFXParticle item = new AVFXParticle();
            item.read( node );
            AVFX.addParticle( item );
            return new UIParticle( item, this );
        }
    }
}
