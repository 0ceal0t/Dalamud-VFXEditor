using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UITextureReflection : UIItem {
        public AVFXTextureReflection Tex;
        public UIParticle Particle;
        //============================
        public UINodeSelect<UITexture> TextureSelect;

        public UITextureReflection(AVFXTextureReflection tex, UIParticle particle) {
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
            Attributes.Add(new UICheckbox("Use Screen Copy", Tex.UseScreenCopy));
            Attributes.Add(new UICombo<TextureFilterType>("Texture Filter", Tex.TextureFilter));
            Attributes.Add(new UICombo<TextureCalculateColor>("Calculate Color", Tex.TextureCalculateColor));
            Attributes.Add( new UICurve( Tex.Rate, "Rate" ) );
            Attributes.Add(new UICurve(Tex.RPow, "Power"));
        }

        // =========== DRAW =====================
        public override void DrawUnAssigned( string parentId )
        {
            if( ImGui.SmallButton( "+ Texture Reflection" + parentId ) )
            {
                Tex.toDefault();
                Init();
            }
        }
        public override void DrawBody( string parentId )
        {
            string id = parentId + "/TR";
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
            return "Texture Reflection";
        }
    }
}
