using ImGuiNET;
using System.Collections.Generic;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Particle;

namespace VFXEditor.AVFX.VFX {
    public class UITextureReflection : UIItem {
        public readonly AVFXParticleTextureReflection Tex;
        public readonly UIParticle Particle;

        public UINodeSelect<UITexture> TextureSelect;

        public readonly List<UIItem> Tabs;
        public readonly UIParameters Parameters;

        public UITextureReflection( AVFXParticleTextureReflection tex, UIParticle particle ) {
            Tex = tex;
            Particle = particle;

            Tabs = new List<UIItem> {
                ( Parameters = new UIParameters( "Parameters" ) )
            };

            if( IsAssigned() ) {
                TextureSelect = new UINodeSelect<UITexture>( Particle, "Texture", Particle.NodeGroups.Textures, Tex.TextureIdx );
            }

            Parameters.Add( new UICheckbox( "Enabled", Tex.Enabled ) );
            Parameters.Add( new UICheckbox( "Use Screen Copy", Tex.UseScreenCopy ) );
            Parameters.Add( new UICombo<TextureFilterType>( "Texture Filter", Tex.TextureFilter ) );
            Parameters.Add( new UICombo<TextureCalculateColor>( "Calculate Color", Tex.TextureCalculateColor ) );

            Tabs.Add( new UICurve( Tex.Rate, "Rate" ) );
            Tabs.Add( new UICurve( Tex.RPow, "Power" ) );
        }

        public override void DrawUnAssigned( string parentId ) {
            if( ImGui.SmallButton( "+ Texture Reflection" + parentId ) ) {
                AVFXBase.RecurseAssigned( Tex, true );

                Parameters.Remove( TextureSelect );
                Parameters.Prepend( TextureSelect = new UINodeSelect<UITexture>( Particle, "Texture", Particle.NodeGroups.Textures, Tex.TextureIdx ) );
            }
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/TR";
            DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => "Texture Reflection";

        public override bool IsAssigned() => Tex.IsAssigned();
    }
}
