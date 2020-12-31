using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIEmitterDataModel : UIBase
    {
        public AVFXEmitterDataModel Data;
        //==========================

        public UIEmitterDataModel(AVFXEmitterDataModel data)
        {
            Data = data;
            //=======================
            Attributes.Add(new UIInt("Model Index", Data.ModelIdx));
            Attributes.Add(new UICombo<RotationOrder>("Rotation Order", Data.RotationOrderType));
            Attributes.Add(new UICombo<GenerateMethod>("Generate Method", Data.GenerateMethodType));
            Attributes.Add(new UICurve(Data.AX, "Angle X"));
            Attributes.Add(new UICurve(Data.AY, "Angle Y"));
            Attributes.Add(new UICurve(Data.AZ, "Angle Z"));
            Attributes.Add(new UICurve(Data.InjectionSpeed, "Injection Speed"));
            Attributes.Add(new UICurve(Data.InjectionSpeedRandom, "Injection Speed Random"));
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
