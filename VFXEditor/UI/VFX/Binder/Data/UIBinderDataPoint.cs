using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIBinderDataPoint : UIBase
    {
        public AVFXBinderDataPoint Data;
        //=======================

        public UIBinderDataPoint(AVFXBinderDataPoint data)
        {
            Data = data;
            //==================
            Attributes.Add(new UICurve(data.SpringStrength, "Spring Strength"));
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Data";
            if (ImGui.TreeNode("Data" + id))
            {
                DrawAttrs(id);
                ImGui.TreePop();
            }
        }
    }
}
