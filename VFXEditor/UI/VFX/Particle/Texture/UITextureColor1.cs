using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UITextureColor1 : UIItem {
        public AVFXTextureColor1 Tex;
        public UIParticle Particle;
        //============================
        public UINodeSelectList<UITexture> TextureSelect;
        public List<UIItem> Tabs;
        public UIParameters Parameters;

        public UITextureColor1(AVFXTextureColor1 tex, UIParticle particle )
        {
            Tex = tex;
            Particle = particle;
            Init();
        }
        public override void Init()
        {
            base.Init();
            if (!Tex.Assigned) { Assigned = false; return; }
            //====================
            Tabs = new List<UIItem>();
            Tabs.Add( Parameters = new UIParameters("Parameters") );

            Parameters.Add( TextureSelect = new UINodeSelectList<UITexture>( Particle, "Mask Texture", UINodeGroup.Textures, Tex.MaskTextureIdx ));
            Parameters.Add(new UICheckbox("Enabled", Tex.Enabled));
            Parameters.Add(new UICheckbox("Color To Alpha", Tex.ColorToAlpha));
            Parameters.Add(new UICheckbox("Use Screen Copy", Tex.UseScreenCopy));
            Parameters.Add(new UICheckbox("Previous Frame Copy", Tex.PreviousFrameCopy));
            Parameters.Add(new UIInt("UV Set Index", Tex.UvSetIdx));
            Parameters.Add(new UICombo<TextureFilterType>("Texture Filter", Tex.TextureFilter));
            Parameters.Add(new UICombo<TextureBorderType>("Texture Border U", Tex.TextureBorderU));
            Parameters.Add(new UICombo<TextureBorderType>("Texture Border V", Tex.TextureBorderV));
            Parameters.Add(new UICombo<TextureCalculateColor>("Calculate Color", Tex.TextureCalculateColor));
            Parameters.Add(new UICombo<TextureCalculateAlpha>("Calculate Alpha", Tex.TextureCalculateAlpha));

            Tabs.Add( new UICurve( Tex.TexN, "TexN" ) );
        }

        // =========== DRAW =====================
        public override void DrawUnAssigned( string parentId )
        {
            if( ImGui.SmallButton( "+ Texture Color 1" + parentId ) )
            {
                Tex.ToDefault();
                Init();
            }
        }
        public override void DrawBody( string parentId )
        {
            var id = parentId + "/TC1";
            if( UIUtils.RemoveButton( "Delete" + id, small: true ) )
            {
                Tex.Assigned = false;
                TextureSelect.DeleteSelect();
                Init();
                return;
            }
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DrawListTabs( Tabs, id );
        }

        public override string GetText() {
            return "Texture Color 1";
        }
    }
}
