using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTextureColor1 : UiAssignableItem {
        public readonly AVFXParticleTextureColor1 Tex;
        public readonly UiParticle Particle;

        public UiNodeSelectList<UiTexture> TextureSelect;

        public readonly List<UiItem> Tabs;
        public readonly UiParameters Parameters;

        public UiTextureColor1( AVFXParticleTextureColor1 tex, UiParticle particle ) {
            Tex = tex;
            Particle = particle;

            Tabs = new List<UiItem> {
                ( Parameters = new UiParameters( "Parameters" ) )
            };

            if( IsAssigned() ) {
                Parameters.Add( TextureSelect = new UiNodeSelectList<UiTexture>( Particle, "Mask Texture", Particle.NodeGroups.Textures, Tex.MaskTextureIdx ) );
            }

            Parameters.Add( new UiCheckbox( "Enabled", Tex.Enabled ) );
            Parameters.Add( new UiCheckbox( "Color To Alpha", Tex.ColorToAlpha ) );
            Parameters.Add( new UiCheckbox( "Use Screen Copy", Tex.UseScreenCopy ) );
            Parameters.Add( new UiCheckbox( "Previous Frame Copy", Tex.PreviousFrameCopy ) );
            Parameters.Add( new UiInt( "UV Set Index", Tex.UvSetIdx ) );
            Parameters.Add( new UiCombo<TextureFilterType>( "Texture Filter", Tex.TextureFilter ) );
            Parameters.Add( new UiCombo<TextureBorderType>( "Texture Border U", Tex.TextureBorderU ) );
            Parameters.Add( new UiCombo<TextureBorderType>( "Texture Border V", Tex.TextureBorderV ) );
            Parameters.Add( new UiCombo<TextureCalculateColor>( "Calculate Color", Tex.TextureCalculateColor ) );
            Parameters.Add( new UiCombo<TextureCalculateAlpha>( "Calculate Alpha", Tex.TextureCalculateAlpha ) );

            Tabs.Add( new UiCurve( Tex.TexN, "TexN" ) );
        }

        public override void DrawUnassigned( string parentId ) {
            if( ImGui.SmallButton( "+ Texture Color 1" + parentId ) ) {
                AVFXBase.RecurseAssigned( Tex, true );

                Parameters.Remove( TextureSelect );
                Parameters.Prepend( TextureSelect = new UiNodeSelectList<UiTexture>( Particle, "Mask Texture", Particle.NodeGroups.Textures, Tex.MaskTextureIdx ) );
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
