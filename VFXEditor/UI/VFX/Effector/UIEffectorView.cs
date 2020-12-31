using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI.VFX
{
    public class UIEffectorView : UIBase
    {
        public AVFXBase AVFX;
        List<UIEffector> Effectors;

        public UIEffectorView(AVFXBase avfx)
        {
            AVFX = avfx;
            Init();
        }
        public override void Init()
        {
            base.Init();
            Effectors = new List<UIEffector>();
            foreach (var effector in AVFX.Effectors)
            {
                Effectors.Add(new UIEffector(effector, this));
            }
        }

        public override void Draw(string parentId = "")
        {
            string id = "##EFFECT";
            int eIdx = 0;
            foreach (var effector in Effectors)
            {
                effector.Idx = eIdx;
                effector.Draw(id);
                eIdx++;
            }
            if (ImGui.Button("+ Effector" + id))
            {
                AVFX.addEffector();
                Init();
            }
        }
    }
}
