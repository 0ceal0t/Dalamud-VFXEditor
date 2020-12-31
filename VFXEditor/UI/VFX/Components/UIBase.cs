using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI.VFX
{
    public abstract class UIBase
    {
        public bool Assigned = true;
        public List<UIBase> Attributes = new List<UIBase>();
        public abstract void Draw(string parentId);
        public void DrawAttrs(string parentId)
        {
            DrawList(Attributes, parentId);
        }

        public virtual void Init()
        {
            Assigned = true;
            Attributes = new List<UIBase>();
        }

        public void DrawList(List<UIBase> items, string parentId)
        {
            int sameLine = 0;
            for(int i = 0; i < items.Count; i++)
            {
                if(i > 0 && !items[i].Assigned && !items[i - 1].Assigned && sameLine < 3){
                    ImGui.SameLine();
                    sameLine++;
                }
                else
                {
                    sameLine = 0;
                }
                items[i].Draw(parentId);
            }
        }
    }
}
