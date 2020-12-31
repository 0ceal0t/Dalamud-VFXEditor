using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIParticleDataPowder: UIBase
    {
        public AVFXParticleDataPowder Data;
        //==========================

        public UIParticleDataPowder(AVFXParticleDataPowder data)
        {
            Data = data;
            Init();
        }
        public override void Init()
        {
            base.Init();
            //=======================
            Attributes.Add(new UICombo<DirectionalLightType>("Directional Light Type", Data.DirectionalLightType));
            Attributes.Add(new UICheckbox("Is Lightning", Data.IsLightning));
            Attributes.Add(new UIFloat("Model Index", Data.CenterOffset));
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
