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
        public List<UIItem> Tabs;
        public UIParameters Parameters;

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
            Tabs = new List<UIItem>();
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );

            TextureSelect = new UINodeSelect<UITexture>( Particle, "Texture", UINodeGroup.Textures, Tex.TextureIdx );
            Parameters.Add(new UICheckbox("Enabled", Tex.Enabled));
            Parameters.Add(new UICheckbox("Use Screen Copy", Tex.UseScreenCopy));
            Parameters.Add(new UICombo<TextureFilterType>("Texture Filter", Tex.TextureFilter));
            Parameters.Add(new UICombo<TextureCalculateColor>("Calculate Color", Tex.TextureCalculateColor));

            Tabs.Add( new UICurve( Tex.Rate, "Rate" ) );
            Tabs.Add(new UICurve(Tex.RPow, "Power"));
        }

        // =========== DRAW =====================
        public override void DrawUnAssigned( string parentId )
        {
            if( ImGui.SmallButton( "+ Texture Reflection" + parentId ) )
            {
                Tex.ToDefault();
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
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DrawListTabs( Tabs, id );
        }

        public override string GetText() {
            return "Texture Reflection";
        }
    }
}
