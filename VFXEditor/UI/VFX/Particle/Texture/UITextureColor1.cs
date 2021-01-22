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
            TextureSelect = new UINodeSelectList<UITexture>( Particle, "Mask Texture", UINode._Textures, Tex.MaskTextureIdx );

            Attributes.Add(new UICheckbox("Enabled", Tex.Enabled));
            Attributes.Add(new UICheckbox("Color To Alpha", Tex.ColorToAlpha));
            Attributes.Add(new UICheckbox("Use Screen Copy", Tex.UseScreenCopy));
            Attributes.Add(new UICheckbox("Previous Frame Copy", Tex.PreviousFrameCopy));
            Attributes.Add(new UIInt("UV Set Index", Tex.UvSetIdx));
            Attributes.Add(new UICombo<TextureFilterType>("Texture Filter", Tex.TextureFilter));
            Attributes.Add(new UICombo<TextureBorderType>("Texture Border U", Tex.TextureBorderU));
            Attributes.Add(new UICombo<TextureBorderType>("Texture Border V", Tex.TextureBorderV));
            Attributes.Add(new UICombo<TextureCalculateColor>("Calculate Color", Tex.TextureCalculateColor));
            Attributes.Add(new UICombo<TextureCalculateAlpha>("Calculate Alpha", Tex.TextureCalculateAlpha));
            Attributes.Add( new UICurve( Tex.TexN, "TexN" ) );
        }

        // =========== DRAW =====================
        public override void DrawUnAssigned( string parentId )
        {
            if( ImGui.SmallButton( "+ Texture Color 1" + parentId ) )
            {
                Tex.toDefault();
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
            TextureSelect.Draw( id );
            DrawAttrs( id );
        }

        public override string GetText() {
            return "Texture Color 1";
        }
    }
}
