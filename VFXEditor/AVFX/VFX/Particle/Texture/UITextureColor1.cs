using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;
using VfxEditor.Utils;

namespace VfxEditor.AVFX.VFX {
    public class UITextureColor1 : UIAssignableItem {
        public readonly AVFXParticleTextureColor1 Tex;
        public readonly UIParticle Particle;

        public UINodeSelectList<UITexture> TextureSelect;

        public readonly List<UIItem> Tabs;
        public readonly UIParameters Parameters;

        public UITextureColor1( AVFXParticleTextureColor1 tex, UIParticle particle ) {
            Tex = tex;
            Particle = particle;

            Tabs = new List<UIItem> {
                ( Parameters = new UIParameters( "Parameters" ) )
            };

            if( IsAssigned() ) {
                Parameters.Add( TextureSelect = new UINodeSelectList<UITexture>( Particle, "Mask Texture", Particle.NodeGroups.Textures, Tex.MaskTextureIdx ) );
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

            Tabs.Add( new UICurve( Tex.TexN, "TexN" ) );
        }

        public override void DrawUnassigned( string parentId ) {
            if( ImGui.SmallButton( "+ Texture Color 1" + parentId ) ) {
                AVFXBase.RecurseAssigned( Tex, true );

                Parameters.Remove( TextureSelect );
                Parameters.Prepend( TextureSelect = new UINodeSelectList<UITexture>( Particle, "Mask Texture", Particle.NodeGroups.Textures, Tex.MaskTextureIdx ) );
            }
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/TC1";
            if( UiUtils.RemoveButton( "Delete Texture Color 1" + id, small: true ) ) {
                Tex.SetAssigned( false );

                TextureSelect.DeleteSelect();
                return;
            }
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => "Texture Color 1";

        public override bool IsAssigned() => Tex.IsAssigned();
    }
}
