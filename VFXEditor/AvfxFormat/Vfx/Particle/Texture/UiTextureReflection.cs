using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTextureReflection : UiParticleAttribute {
        public readonly AVFXParticleTextureReflection Tex;

        public UiTextureReflection( AVFXParticleTextureReflection tex, UiParticle particle ) : base( particle ) {
            Tex = tex;
            InitNodeSelects();

            Parameters.Add( new UiCheckbox( "Enabled", Tex.Enabled ) );
            Parameters.Add( new UiCheckbox( "Use Screen Copy", Tex.UseScreenCopy ) );
            Parameters.Add( new UiCombo<TextureFilterType>( "Texture Filter", Tex.TextureFilter ) );
            Parameters.Add( new UiCombo<TextureCalculateColor>( "Calculate Color", Tex.TextureCalculateColor ) );

            Tabs.Add( new UiCurve( Tex.Rate, "Rate" ) );
            Tabs.Add( new UiCurve( Tex.RPow, "Power" ) );
        }

        public override void DrawUnassigned( string parentId ) {
            if( ImGui.SmallButton( "+ Texture Reflection" + parentId ) ) Assign( Tex );
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/TR";
            DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => "Texture Reflection";

        public override bool IsAssigned() => Tex.IsAssigned();

        public override List<UiNodeSelect> GetNodeSelects() => new() {
            new UiNodeSelect<UiTexture>( Particle, "Texture", Particle.NodeGroups.Textures, Tex.TextureIdx )
        };
    }
}
