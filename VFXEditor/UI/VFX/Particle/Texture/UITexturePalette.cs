using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UITexturePalette : UIItem {
        public AVFXTexturePalette Tex;
        public UIParticle Particle;
        public string Name;
        //============================
        public UINodeSelect<UITexture> TextureSelect;
        public List<UIItem> Tabs;
        public UIParameters Parameters;

        public UITexturePalette(AVFXTexturePalette tex, UIParticle particle )
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
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );

            Parameters.Add(TextureSelect = new UINodeSelect<UITexture>( Particle, "Texture", Particle.Main.Textures, Tex.TextureIdx ));
            Parameters.Add(new UICheckbox("Enabled", Tex.Enabled));
            Parameters.Add(new UICombo<TextureFilterType>("Texture Filter", Tex.TextureFilter));
            Parameters.Add(new UICombo<TextureBorderType>("Texture Border", Tex.TextureBorder));

            Tabs.Add( new UICurve( Tex.Offset, "Offset" ) );
        }

        // =========== DRAW =====================
        public override void DrawUnAssigned( string parentId )
        {
            if( ImGui.SmallButton( "+ Texture Palette" + parentId ) )
            {
                Tex.ToDefault();
                Init();
            }
        }
        public override void DrawBody( string parentId )
        {
            var id = parentId + "/TP";
            if( UIUtils.RemoveButton( "Delete Texture Palette" + id, small: true ) )
            {
                Tex.Assigned = false;
                TextureSelect.DeleteSelect();
                Init();
                return;
            }
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() {
            return "Texture Palette";
        }
    }
}
