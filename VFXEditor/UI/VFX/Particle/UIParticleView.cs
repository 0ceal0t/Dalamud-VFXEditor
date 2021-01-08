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
    public class UIParticleView : UIDropdownView
    {
        public AVFXBase AVFX;
        List<UIParticle> Particles;

        public UIParticleView( AVFXBase avfx ) : base( "##PTCL", "Select a Particle" )
        {
            AVFX = avfx;
            Init();
        }

        public override void Init()
        {
            base.Init();
            Particles = new List<UIParticle>();
            Options = new string[AVFX.Particles.Count];
            int idx = 0;
            foreach (var particle in AVFX.Particles)
            {
                var item = new UIParticle( particle, this );
                item.Idx = idx;
                Options[idx] = item.GetDescText();
                Particles.Add( item );
                idx++;
            }
        }

        public override void OnNew()
        {
            AVFX.addParticle();
        }
        public override void OnDelete( int idx )
        {
            AVFX.removeParticle( idx );
        }
        public override void OnDraw( int idx )
        {
            if( idx >= Particles.Count ) return;
            Particles[idx].Draw( id );
        }
        public override byte[] OnExport( int idx )
        {
            return Particles[idx].Particle.toAVFX().toBytes();
        }
        public override void RefreshDesc( int idx )
        {
            Options[idx] = Particles[idx].GetDescText();
        }
        public override void OnImport( AVFXNode node ) {
            AVFXParticle item = new AVFXParticle();
            item.read( node );
            AVFX.addParticle( item );
        }
    }
}
