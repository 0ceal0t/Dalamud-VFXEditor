using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UITextureDistortion : UIItem {
        public AVFXTextureDistortion Tex;
        public UIParticle Particle;
        public string Name;
        //============================
        public UINodeSelect<UITexture> TextureSelect;
        public List<UIItem> Tabs;
        public UIParameters Parameters;

        public UITextureDistortion(AVFXTextureDistortion tex, UIParticle particle ) {
            Tex = tex;
            Particle = particle;
            Init();
        }

        public override void Init() {
            base.Init();
            if (!Tex.Assigned) { Assigned = false; return; }
            //====================
            Tabs = new List<UIItem>();
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );

            Parameters.Add( TextureSelect = new UINodeSelect<UITexture>( Particle, "Texture", Particle.Main.Textures, Tex.TextureIdx ));
            Parameters.Add(new UICheckbox("Enabled", Tex.Enabled));
            Parameters.Add(new UICheckbox("Distort UV1", Tex.TargetUV1));
            Parameters.Add(new UICheckbox("Distort UV2", Tex.TargetUV2));
            Parameters.Add(new UICheckbox("Distort UV3", Tex.TargetUV3));
            Parameters.Add(new UICheckbox("Distort UV4", Tex.TargetUV4));
            Parameters.Add(new UIInt("UV Set Index", Tex.UvSetIdx));
            Parameters.Add(new UICombo<TextureFilterType>("Texture Filter", Tex.TextureFilter));
            Parameters.Add(new UICombo<TextureBorderType>("Texture Border U", Tex.TextureBorderU));
            Parameters.Add(new UICombo<TextureBorderType>("Texture Border V", Tex.TextureBorderV));

            Tabs.Add(new UICurve(Tex.DPow, "Power"));
        }

        // =========== DRAW =====================
        public override void DrawUnAssigned( string parentId )
        {
            if( ImGui.SmallButton( "+ Texture Distortion" + parentId ) )
            {
                Tex.ToDefault();
                Init();
            }
        }
        public override void DrawBody( string parentId )
        {
            string id = parentId + "/TD";
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

        public override string GetDefaultText( ) {
            return "Texture Distortion";
        }
    }
}
