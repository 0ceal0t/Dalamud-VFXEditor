using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UITextureColor1 : UIBase
    {
        public AVFXTextureColor1 Tex;
        //============================

        public UITextureColor1(AVFXTextureColor1 tex)
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
            Attributes.Add(new UICheckbox("Color To Alpha", Tex.ColorToAlpha));
            Attributes.Add(new UICheckbox("Use Screen Copy", Tex.UseScreenCopy));
            Attributes.Add(new UICheckbox("Previous Frame Copy", Tex.PreviousFrameCopy));
            Attributes.Add(new UIInt("UV Set Index", Tex.UvSetIdx));
            Attributes.Add(new UIInt("Mask Texture Index", Tex.MaskTextureIdx));
            Attributes.Add(new UICombo<TextureFilterType>("Texture Filter", Tex.TextureFilter));
            Attributes.Add(new UICombo<TextureBorderType>("Texture Border U", Tex.TextureBorderU));
            Attributes.Add(new UICombo<TextureBorderType>("Texture Border V", Tex.TextureBorderV));
            Attributes.Add(new UICombo<TextureCalculateColor>("Calculate Color", Tex.TextureCalculateColor));
            Attributes.Add(new UICombo<TextureCalculateAlpha>("Calculate Alpha", Tex.TextureCalculateAlpha));
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/TC1";
            // === UNASSIGNED ===
            if (!Assigned)
            {
                if (ImGui.Button("+ Texture Color 1" + id))
                {
                    Tex.toDefault();
                    Init();
                }
                return;
            }
            // ==== ASSIGNED ===
            if (ImGui.TreeNode("Texture Color 1" + id))
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
