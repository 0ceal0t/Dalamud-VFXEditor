using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UITextureColor2 : UIItem {
        public AVFXTextureColor2 Tex;
        public UIParticle Particle;
        public string Name;
        //============================
        public UINodeSelect<UITexture> TextureSelect;
        public List<UIItem> Tabs;
        public UIParameters Parameters;

        public UITextureColor2(AVFXTextureColor2 tex, string name, UIParticle particle )
        {
            Tex = tex;
            Name = name;
            Particle = particle;
            Init();
        }
        public override void Init()
        {
            base.Init();
            if (!Tex.Assigned) { Assigned = false; return; }
            //====================
            Tabs = new List<UIItem> {
                ( Parameters = new UIParameters( "Parameters" ) )
            };

            Parameters.Add( TextureSelect = new UINodeSelect<UITexture>( Particle, "Texture", Particle.Main.Textures, Tex.TextureIdx ));
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
        }

        // =========== DRAW =====================
        public override void DrawUnAssigned( string parentId )  {
            if( ImGui.SmallButton( "+ " + Name + parentId ) ) {
                Tex.ToDefault();
                Init();
            }
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/" + Name;
            if( UIUtils.RemoveButton( "Delete " + Name + id, small: true ) ) {
                Tex.Assigned = false;
                TextureSelect.DeleteSelect();
                Init();
                return;
            }
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText( ) {
            return Name;
        }
    }
}
