using ImGuiNET;
using System;
using System.Collections.Generic;
using VFXEditor.Helper;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Particle;

namespace VFXEditor.AVFX.VFX {
    public class UITextureColor2 : UIItem {
        public readonly AVFXParticleTextureColor2 Tex;
        public readonly UIParticle Particle;
        public readonly string Name;

        public UINodeSelect<UITexture> TextureSelect;

        public readonly List<UIItem> Tabs;
        public readonly UIParameters Parameters;

        public UITextureColor2( AVFXParticleTextureColor2 tex, string name, UIParticle particle ) {
            Tex = tex;
            Name = name;
            Particle = particle;

            Tabs = new List<UIItem> {
                ( Parameters = new UIParameters( "Parameters" ) )
            };

            if( IsAssigned() ) {
                Parameters.Add( TextureSelect = new UINodeSelect<UITexture>( Particle, "Texture", Particle.Main.Textures, Tex.TextureIdx ) );
            }

            Parameters.Add( new UICheckbox( "Enabled", Tex.Enabled ) );
            Parameters.Add( new UICheckbox( "Color To Alpha", Tex.ColorToAlpha ) );
            Parameters.Add( new UICheckbox( "Use Screen Copy", Tex.UseScreenCopy ) );
            Parameters.Add( new UICheckbox( "Previous Frame Copy", Tex.PreviousFrameCopy ) );
            Parameters.Add( new UIInt( "UV Set Index", Tex.UvSetIdx ) );
            Parameters.Add( new UICombo<TextureFilterType>( "Texture Filter", Tex.TextureFilter ) );
            Parameters.Add( new UICombo<TextureBorderType>( "Texture Border U", Tex.TextureBorderU ) );
            Parameters.Add( new UICombo<TextureBorderType>( "Texture Border V", Tex.TextureBorderV ) );
            Parameters.Add( new UICombo<TextureCalculateColor>( "Calculate Color", Tex.TextureCalculateColor ) );
            Parameters.Add( new UICombo<TextureCalculateAlpha>( "Calculate Alpha", Tex.TextureCalculateAlpha ) );
        }

        public override void DrawUnAssigned( string parentId ) {
            if( ImGui.SmallButton( "+ " + Name + parentId ) ) {
                AVFXBase.RecurseAssigned( Tex, true );

                Parameters.Remove( TextureSelect );
                Parameters.Prepend( TextureSelect = new UINodeSelect<UITexture>( Particle, "Texture", Particle.Main.Textures, Tex.TextureIdx ) );
            }
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/" + Name;

            if( Tex.GetName() != "TC2" && UIHelper.RemoveButton( "Delete " + Name + id, small: true ) ) {
                Tex.SetAssigned( false );

                TextureSelect.DeleteSelect();
                return;
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => Name;

        public override bool IsAssigned() => Tex.IsAssigned();
    }
}
