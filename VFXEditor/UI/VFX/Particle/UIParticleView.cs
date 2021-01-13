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
        public AVFXBase AVFX;

        public UIParticleView( AVFXBase avfx ) : base( "##PTCL", "Select a Particle" )
        {
            AVFX = avfx;
            //===============
            foreach( var particle in AVFX.Particles ) {
                var item = new UIParticle( particle, this );
                Items.Add( item );
            }
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
