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
    public class UIParticleView : UIDropdownView<UIParticle> {
        public UIParticleView( UIMain main, AVFXBase avfx ) : base( main, avfx, "##PTCL", "Select a Particle", defaultPath: "particle_default.vfxedit" ) {
            Group = Main.Particles;
            Group.Items = AVFX.Particles.Select( item => new UIParticle( Main, item ) ).ToList();
        }

        public override void OnDelete( UIParticle item ) {
            AVFX.RemoveParticle( item.Particle );
        }

        public override byte[] OnExport( UIParticle item ) {
            return item.Particle.ToAVFX().ToBytes();
        }

        public override UIParticle OnImport( AVFXNode node, bool has_dependencies = false ) {
            AVFXParticle item = new AVFXParticle();
            item.Read( node );
            AVFX.AddParticle( item );
            return new UIParticle( Main, item, has_dependencies );
        }
    }
}
