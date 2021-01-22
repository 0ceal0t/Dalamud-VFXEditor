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

        public UITextureDistortion(AVFXTextureDistortion tex, UIParticle particle )
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
            TextureSelect = new UINodeSelect<UITexture>( Particle, "Texture", UINode._Textures, Tex.TextureIdx );

            Attributes.Add(new UICheckbox("Enabled", Tex.Enabled));
            Attributes.Add(new UICheckbox("Distort UV1", Tex.TargetUV1));
            Attributes.Add(new UICheckbox("Distort UV2", Tex.TargetUV2));
            Attributes.Add(new UICheckbox("Distort UV3", Tex.TargetUV3));
            Attributes.Add(new UICheckbox("Distort UV4", Tex.TargetUV4));
            Attributes.Add(new UIInt("UV Set Index", Tex.UvSetIdx));
            Attributes.Add(new UICombo<TextureFilterType>("Texture Filter", Tex.TextureFilter));
            Attributes.Add(new UICombo<TextureBorderType>("Texture Border U", Tex.TextureBorderU));
            Attributes.Add(new UICombo<TextureBorderType>("Texture Border V", Tex.TextureBorderV));
            Attributes.Add(new UICurve(Tex.DPow, "Power"));
        }

        // =========== DRAW =====================
        public override void DrawUnAssigned( string parentId )
        {
            if( ImGui.SmallButton( "+ Texture Distortion" + parentId ) )
            {
                Tex.toDefault();
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
            TextureSelect.Draw( id );
            DrawAttrs( id );
        }

        public override string GetText( ) {
            return "Texture Distortion";
        }
    }
}
