using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTexturePalette : UiAssignableItem {
        public readonly AVFXParticleTexturePalette Tex;
        public readonly UiParticle Particle;
        public readonly string Name;

        public UiNodeSelect<UiTexture> TextureSelect;

        public readonly List<UiItem> Tabs;
        public readonly UiParameters Parameters;

        public UiTexturePalette( AVFXParticleTexturePalette tex, UiParticle particle ) {
            Tex = tex;
            Particle = particle;

            Tabs = new List<UiItem> {
                ( Parameters = new UiParameters( "Parameters" ) )
            };

            if( IsAssigned() ) {
                Parameters.Add( TextureSelect = new UiNodeSelect<UiTexture>( Particle, "Texture", Particle.NodeGroups.Textures, Tex.TextureIdx ) );
            }

            Parameters.Add( new UiCheckbox( "Enabled", Tex.Enabled ) );
            Parameters.Add( new UiCombo<TextureFilterType>( "Texture Filter", Tex.TextureFilter ) );
            Parameters.Add( new UiCombo<TextureBorderType>( "Texture Border", Tex.TextureBorder ) );

            Tabs.Add( new UiCurve( Tex.Offset, "Offset" ) );
        }

        public override void DrawUnassigned( string parentId ) {
            if( ImGui.SmallButton( "+ Texture Palette" + parentId ) ) {
                AVFXBase.RecurseAssigned( Tex, true );

                Parameters.Remove( TextureSelect );
                Parameters.Prepend( TextureSelect = new UiNodeSelect<UiTexture>( Particle, "Texture", Particle.NodeGroups.Textures, Tex.TextureIdx ) );
            }
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/TP";
            DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => "Texture Palette";

        public override bool IsAssigned() => Tex.IsAssigned();
    }
}
