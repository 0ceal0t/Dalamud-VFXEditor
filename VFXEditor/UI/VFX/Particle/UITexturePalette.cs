using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UITexturePalette : UIBase
    {
        public AVFXTexturePalette Tex;
        public string Name;
        //============================

        public UITexturePalette(AVFXTexturePalette tex)
        {
            Tex = tex;
            Init();
        }
        public override void Init()
        {
            base.Init();
            if (!Tex.Assigned) { Assigned = false; return; }
            //====================
            Attributes.Add(new UICheckbox("Enabled", Tex.Enabled));
            Attributes.Add(new UIInt("Texture Index", Tex.TextureIdx));
            Attributes.Add(new UICombo<TextureFilterType>("Texture Filter", Tex.TextureFilter));
            Attributes.Add(new UICombo<TextureBorderType>("Texture Border", Tex.TextureBorder));
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/TP";
            // === UNASSIGNED ===
            if (!Assigned)
            {
                if (ImGui.Button("+ Texture Palette" + id))
                {
                    Tex.toDefault();
                    Init();
                }
                return;
            }
            // ==== ASSIGNED ===
            if (ImGui.TreeNode("Palette" + id))
            {
                if (UIUtils.RemoveButton("Delete " + id))
                {
                    Tex.Assigned = false;
                    Init();
                }
                DrawAttrs(id);
                ImGui.TreePop();
            }
        }
    }
}
