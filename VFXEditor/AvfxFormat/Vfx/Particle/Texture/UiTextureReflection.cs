using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTextureReflection : UiAssignableItem {
        public readonly AVFXParticleTextureReflection Tex;
        public readonly UiParticle Particle;

        public UiNodeSelect<UiTexture> TextureSelect;

        public readonly List<UiItem> Tabs;
        public readonly UiParameters Parameters;

        public UiTextureReflection( AVFXParticleTextureReflection tex, UiParticle particle ) {
            Tex = tex;
            Particle = particle;

            Tabs = new List<UiItem> {
                ( Parameters = new UiParameters( "Parameters" ) )
            };

            if( IsAssigned() ) {
                TextureSelect = new UiNodeSelect<UiTexture>( Particle, "Texture", Particle.NodeGroups.Textures, Tex.TextureIdx );
            }

            Parameters.Add( new UiCheckbox( "Enabled", Tex.Enabled ) );
            Parameters.Add( new UiCheckbox( "Use Screen Copy", Tex.UseScreenCopy ) );
            Parameters.Add( new UiCombo<TextureFilterType>( "Texture Filter", Tex.TextureFilter ) );
            Parameters.Add( new UiCombo<TextureCalculateColor>( "Calculate Color", Tex.TextureCalculateColor ) );

            Tabs.Add( new UiCurve( Tex.Rate, "Rate" ) );
            Tabs.Add( new UiCurve( Tex.RPow, "Power" ) );
        }

        public override void DrawUnassigned( string parentId ) {
            if( ImGui.SmallButton( "+ Texture Reflection" + parentId ) ) {
                AVFXBase.RecurseAssigned( Tex, true );

                Parameters.Remove( TextureSelect );
                Parameters.Prepend( TextureSelect = new UiNodeSelect<UiTexture>( Particle, "Texture", Particle.NodeGroups.Textures, Tex.TextureIdx ) );
            }
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/TR";
            DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => "Texture Reflection";

        public override bool IsAssigned() => Tex.IsAssigned();
    }
}
