using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI.VFX
{
    public class UIBinderView : UIBase
    {
        public AVFXBase AVFX;
        List<UIBinder> Binders;

        public UIBinderView(AVFXBase avfx)
        {
            AVFX = avfx;
            Init();
        }
        public override void Init()
        {
            base.Init();
            Binders = new List<UIBinder>();
            foreach (var binder in AVFX.Binders)
            {
                Binders.Add(new UIBinder(binder, this));
            }
        }

        public override void Draw(string parentId = "")
        {
            string id = "##BIND";
            int bIdx = 0;
            foreach (var binder in Binders)
            {
                binder.Idx = bIdx;
                binder.Draw(id);
                bIdx++;
            }
            if (ImGui.Button("+ Binder" + id))
            {
                AVFX.addBinder();
                Init();
            }
        }
    }
}
