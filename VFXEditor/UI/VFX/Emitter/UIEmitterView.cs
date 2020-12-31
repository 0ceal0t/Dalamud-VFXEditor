using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI.VFX
{
    public class UIEmitterView : UIBase
    {
        public AVFXBase AVFX;
        List<UIEmitter> Emitters;

        public UIEmitterView(AVFXBase avfx)
        {
            AVFX = avfx;
            Init();
        }
        public override void Init()
        {
            base.Init();
            Emitters = new List<UIEmitter>();
            foreach (var emitter in AVFX.Emitters)
            {
                Emitters.Add(new UIEmitter(emitter, this));
            }
        }

        public override void Draw(string parentId = "")
        {
            string id = "##EMIT";
            int eIdx = 0;
            foreach (var emitter in Emitters)
            {
                emitter.Idx = eIdx;
                emitter.Draw(id);
                eIdx++;
            }
            if (ImGui.Button("+ Emitter" + id))
            {
                AVFX.addEmitter();
                Init();
            }
        }
    }
}
