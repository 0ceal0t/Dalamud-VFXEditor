using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI.VFX
{
    public class UIParticleView : UIBase
    {
        public AVFXBase AVFX;
        List<UIParticle> Particles;
        public int Selected = -1;
        public string[] Options;

        public UIParticleView(AVFXBase avfx)
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
        public void RefreshDesc(int idx)
        {
            Options[idx] = Particles[idx].GetDescText();
        }
        public override void Draw(string parentId = "")
        {
            string id = "##PTCL";
            bool validSelect = UIUtils.ViewSelect( id, "Select a Particle", ref Selected, Options );
            ImGui.SameLine();
            if( ImGui.Button( "+ NEW" + id ) )
            {
                AVFX.addParticle();
                Init();
            }
            if( validSelect )
            {
                ImGui.SameLine();
                if( UIUtils.RemoveButton( "DELETE" + id ) )
                {
                    AVFX.removeParticle( Selected );
                    Init();
                    validSelect = false;
                }
            }
            ImGui.Separator();
            // ====================
            if( validSelect )
            {
                Particles[Selected].Draw( id );
            }
        }
    }
}
