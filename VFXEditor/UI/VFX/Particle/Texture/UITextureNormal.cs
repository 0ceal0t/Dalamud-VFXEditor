using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UITextureNormal : UIItem {
        public AVFXTextureNormal Tex;
        public string Name;
        //============================

        public UITextureNormal(AVFXTextureNormal tex)
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
            Attributes.Add(new UIInt("UV Set Index", Tex.UvSetIdx));
            Attributes.Add(new UIInt("Texture Index", Tex.TextureIdx));
            Attributes.Add(new UICombo<TextureFilterType>("Texture Filter", Tex.TextureFilter));
            Attributes.Add(new UICombo<TextureBorderType>("Texture Border U", Tex.TextureBorderU));
            Attributes.Add(new UICombo<TextureBorderType>("Texture Border V", Tex.TextureBorderV));
            Attributes.Add(new UICurve(Tex.NPow, "Power"));
        }

        // =========== DRAW =====================
        public override void DrawUnAssigned( string parentId )
        {
            if( ImGui.SmallButton( "+ Texture Normal" + parentId ) )
            {
                Tex.toDefault();
                Init();
            }
        }
        public override void DrawBody( string parentId )
        {
            string id = parentId + "/TN";
            if( UIUtils.RemoveButton( "Delete" + id, small: true ) )
            {
                Tex.Assigned = false;
                Init();
                return;
            }
            DrawAttrs( id );
        }

        public override string GetText( ) {
            return "Texture Normal";
        }
    }
}
