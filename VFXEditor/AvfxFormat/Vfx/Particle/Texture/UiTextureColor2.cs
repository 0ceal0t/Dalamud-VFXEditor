using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTextureColor2 : UiAssignableItem {
        public readonly AVFXParticleTextureColor2 Tex;
        public readonly UiParticle Particle;
        public readonly string Name;

        public UiNodeSelect<UiTexture> TextureSelect;

        public readonly List<UiItem> Tabs;
        public readonly UiParameters Parameters;

        public UiTextureColor2( AVFXParticleTextureColor2 tex, string name, UiParticle particle ) {
            Tex = tex;
            Name = name;
            Particle = particle;

            Tabs = new List<UiItem> {
                ( Parameters = new UiParameters( "Parameters" ) )
            };

            if( IsAssigned() ) {
                Parameters.Add( TextureSelect = new UiNodeSelect<UiTexture>( Particle, "Texture", Particle.NodeGroups.Textures, Tex.TextureIdx ) );
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
        }

        public override void DrawUnassigned( string parentId ) {
            if( ImGui.SmallButton( "+ " + Name + parentId ) ) {
                AVFXBase.RecurseAssigned( Tex, true );

                Parameters.Remove( TextureSelect );
                Parameters.Prepend( TextureSelect = new UiNodeSelect<UiTexture>( Particle, "Texture", Particle.NodeGroups.Textures, Tex.TextureIdx ) );
            }
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/" + Name;

            if( Tex.GetName() != "TC2" && UiUtils.RemoveButton( "Delete " + Name + id, small: true ) ) {
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
