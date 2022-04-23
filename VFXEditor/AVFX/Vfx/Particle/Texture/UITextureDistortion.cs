using ImGuiNET;
using System;
using System.Collections.Generic;
using VFXEditor.Helper;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Particle;

namespace VFXEditor.AVFX.VFX {
    public class UITextureDistortion : UIItem {
        public readonly AVFXParticleTextureDistortion Tex;
        public readonly UIParticle Particle;
        public readonly string Name;

        public UINodeSelect<UITexture> TextureSelect;

        public readonly List<UIItem> Tabs;
        public readonly UIParameters Parameters;

        public UITextureDistortion( AVFXParticleTextureDistortion tex, UIParticle particle ) {
            Tex = tex;
            Particle = particle;

            Tabs = new List<UIItem> {
                ( Parameters = new UIParameters( "Parameters" ) )
            };

            if ( IsAssigned() ) {
                Parameters.Add( TextureSelect = new UINodeSelect<UITexture>( Particle, "Texture", Particle.Main.Textures, Tex.TextureIdx ) );
            }

            Parameters.Add( new UICheckbox( "Enabled", Tex.Enabled ) );
            Parameters.Add( new UICheckbox( "Distort UV1", Tex.TargetUV1 ) );
            Parameters.Add( new UICheckbox( "Distort UV2", Tex.TargetUV2 ) );
            Parameters.Add( new UICheckbox( "Distort UV3", Tex.TargetUV3 ) );
            Parameters.Add( new UICheckbox( "Distort UV4", Tex.TargetUV4 ) );
            Parameters.Add( new UIInt( "UV Set Index", Tex.UvSetIdx ) );
            Parameters.Add( new UICombo<TextureFilterType>( "Texture Filter", Tex.TextureFilter ) );
            Parameters.Add( new UICombo<TextureBorderType>( "Texture Border U", Tex.TextureBorderU ) );
            Parameters.Add( new UICombo<TextureBorderType>( "Texture Border V", Tex.TextureBorderV ) );

            Tabs.Add( new UICurve( Tex.DPow, "Power" ) );
        }

        public override void DrawUnAssigned( string parentId ) {
            if( ImGui.SmallButton( "+ Texture Distortion" + parentId ) ) {
                AVFXBase.RecurseAssigned( Tex, true );

                Parameters.Remove( TextureSelect );
                Parameters.Prepend( TextureSelect = new UINodeSelect<UITexture>( Particle, "Texture", Particle.Main.Textures, Tex.TextureIdx ) );
            }
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/TD";
            DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => "Texture Distortion";

        public override bool IsAssigned() => Tex.IsAssigned();
    }
}
