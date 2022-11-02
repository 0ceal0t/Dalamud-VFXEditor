using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTextureNormal : UiAssignableItem {
        public readonly AVFXParticleTextureNormal Tex;
        public readonly UiParticle Particle;
        public readonly string Name;

        public UiNodeSelect<UiTexture> TextureSelect;

        public readonly List<UiItem> Tabs;
        public readonly UiParameters Parameters;

        public UiTextureNormal( AVFXParticleTextureNormal tex, UiParticle particle ) {
            Tex = tex;
            Particle = particle;

            Tabs = new List<UiItem> {
                ( Parameters = new UiParameters( "Parameters" ) )
            };

            if( IsAssigned() ) {
                Parameters.Add( TextureSelect = new UiNodeSelect<UiTexture>( Particle, "Texture", Particle.NodeGroups.Textures, Tex.TextureIdx ) );
            }

            Parameters.Add( new UiCheckbox( "Enabled", Tex.Enabled ) );
            Parameters.Add( new UiInt( "UV Set Index", Tex.UvSetIdx ) );
            Parameters.Add( new UiCombo<TextureFilterType>( "Texture Filter", Tex.TextureFilter ) );
            Parameters.Add( new UiCombo<TextureBorderType>( "Texture Border U", Tex.TextureBorderU ) );
            Parameters.Add( new UiCombo<TextureBorderType>( "Texture Border V", Tex.TextureBorderV ) );

            Tabs.Add( new UiCurve( Tex.NPow, "Power" ) );
        }

        public override void DrawUnassigned( string parentId ) {
            if( ImGui.SmallButton( "+ Texture Normal" + parentId ) ) {
                AVFXBase.RecurseAssigned( Tex, true );

                Parameters.Remove( TextureSelect );
                Parameters.Prepend( TextureSelect = new UiNodeSelect<UiTexture>( Particle, "Texture", Particle.NodeGroups.Textures, Tex.TextureIdx ) );
            }
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/TN";
            DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => "Texture Normal";

        public override bool IsAssigned() => Tex.IsAssigned();
    }
}
