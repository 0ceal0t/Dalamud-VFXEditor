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

        public UIParticleView(AVFXBase avfx)
        {
            AVFX = avfx;
            Init();
        }
        public override void Init()
        {
            base.Init();
            Particles = new List<UIParticle>();
            foreach (var particle in AVFX.Particles)
            {
                Particles.Add(new UIParticle(particle, this));
            }
        }

        public override void Draw(string parentId = "")
        {
            string id = "##PTCL";
            int pIdx = 0;
            foreach(var particle in Particles)
            {
                particle.Idx = pIdx;
                particle.Draw(id);
                pIdx++;
            }
            if (ImGui.Button("+ Particle" + id))
            {
                AVFX.addParticle();
                Init();
            }
        }
    }
}
