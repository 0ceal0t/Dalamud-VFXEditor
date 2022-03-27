using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using VFXEditor.Helper;

namespace VFXEditor.Avfx.Vfx {
    public class UITextureReflection : UIItem {
        public AVFXTextureReflection Tex;
        public UIParticle Particle;
        public UINodeSelect<UITexture> TextureSelect;
        public List<UIItem> Tabs;
        public UIParameters Parameters;

        public UITextureReflection( AVFXTextureReflection tex, UIParticle particle ) {
            Tex = tex;
            Particle = particle;
            Init();
        }

        public override void Init() {
            base.Init();
            if( !Tex.Assigned ) { Assigned = false; return; }
            //====================
            Tabs = new List<UIItem> {
                ( Parameters = new UIParameters( "Parameters" ) )
            };

            TextureSelect = new UINodeSelect<UITexture>( Particle, "Texture", Particle.Main.Textures, Tex.TextureIdx );
            Parameters.Add( new UICheckbox( "Enabled", Tex.Enabled ) );
            Parameters.Add( new UICheckbox( "Use Screen Copy", Tex.UseScreenCopy ) );
            Parameters.Add( new UICombo<TextureFilterType>( "Texture Filter", Tex.TextureFilter ) );
            Parameters.Add( new UICombo<TextureCalculateColor>( "Calculate Color", Tex.TextureCalculateColor ) );

            Tabs.Add( new UICurve( Tex.Rate, "Rate" ) );
            Tabs.Add( new UICurve( Tex.RPow, "Power" ) );
        }

        public override void DrawUnAssigned( string parentId ) {
            if( ImGui.SmallButton( "+ Texture Reflection" + parentId ) ) {
                Tex.ToDefault();
                Init();
            }
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/TR";
            if( UiHelper.RemoveButton( "Delete Texture Reflection" + id, small: true ) ) {
                Tex.Assigned = false;
                TextureSelect.DeleteSelect();
                Init();
                return;
            }
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => "Texture Reflection";
    }
}
