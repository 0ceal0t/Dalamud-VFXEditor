using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTextureDistortion : UiAssignableItem {
        public readonly AVFXParticleTextureDistortion Tex;
        public readonly UiParticle Particle;
        public readonly string Name;

        public UiNodeSelect<UiTexture> TextureSelect;

        public readonly List<UiItem> Tabs;
        public readonly UiParameters Parameters;

        public UiTextureDistortion( AVFXParticleTextureDistortion tex, UiParticle particle ) {
            Tex = tex;
            Particle = particle;

            Tabs = new List<UiItem> {
                ( Parameters = new UiParameters( "Parameters" ) )
            };

            if( IsAssigned() ) {
                Parameters.Add( TextureSelect = new UiNodeSelect<UiTexture>( Particle, "Texture", Particle.NodeGroups.Textures, Tex.TextureIdx ) );
            }

            Parameters.Add( new UiCheckbox( "Enabled", Tex.Enabled ) );
            Parameters.Add( new UiCheckbox( "Distort UV1", Tex.TargetUV1 ) );
            Parameters.Add( new UiCheckbox( "Distort UV2", Tex.TargetUV2 ) );
            Parameters.Add( new UiCheckbox( "Distort UV3", Tex.TargetUV3 ) );
            Parameters.Add( new UiCheckbox( "Distort UV4", Tex.TargetUV4 ) );
            Parameters.Add( new UiInt( "UV Set Index", Tex.UvSetIdx ) );
            Parameters.Add( new UiCombo<TextureFilterType>( "Texture Filter", Tex.TextureFilter ) );
            Parameters.Add( new UiCombo<TextureBorderType>( "Texture Border U", Tex.TextureBorderU ) );
            Parameters.Add( new UiCombo<TextureBorderType>( "Texture Border V", Tex.TextureBorderV ) );

            Tabs.Add( new UiCurve( Tex.DPow, "Power" ) );
        }

        public override void DrawUnassigned( string parentId ) {
            if( ImGui.SmallButton( "+ Texture Distortion" + parentId ) ) {
                AVFXBase.RecurseAssigned( Tex, true );

                Parameters.Remove( TextureSelect );
                Parameters.Prepend( TextureSelect = new UiNodeSelect<UiTexture>( Particle, "Texture", Particle.NodeGroups.Textures, Tex.TextureIdx ) );
            }
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/TD";
            DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => "Texture Distortion";

        public override bool IsAssigned() => Tex.IsAssigned();
    }
}
