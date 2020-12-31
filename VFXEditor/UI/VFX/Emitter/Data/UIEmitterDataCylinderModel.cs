using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIEmitterDataCylinderModel : UIBase
    {
        public AVFXEmitterDataCylinderModel Data;
        //==========================

        public UIEmitterDataCylinderModel(AVFXEmitterDataCylinderModel data)
        {
            Data = data;
            //=======================
            Attributes.Add(new UICombo<RotationOrder>("Rotation Order", Data.RotationOrderType));
            Attributes.Add(new UICombo<GenerateMethod>("Generate Method", Data.GenerateMethodType));
            Attributes.Add(new UIInt("Divide X", Data.DivideX));
            Attributes.Add(new UIInt("Divide Y", Data.DivideY));
            Attributes.Add(new UICurve(Data.Radius, "Radius"));
            Attributes.Add(new UICurve(Data.Length, "Length"));
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
