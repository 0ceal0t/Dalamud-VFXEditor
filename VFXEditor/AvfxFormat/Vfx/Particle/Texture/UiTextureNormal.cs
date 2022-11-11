using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTextureNormal : UiTextureItem {
        public readonly AVFXParticleTextureNormal Tex;

        public UiTextureNormal( AVFXParticleTextureNormal tex, UiParticle particle ) : base( particle ) {
            Tex = tex;
            InitNodeSelects();

            Parameters.Add( new UiCheckbox( "Enabled", Tex.Enabled ) );
            Parameters.Add( new UiInt( "UV Set Index", Tex.UvSetIdx ) );
            Parameters.Add( new UiCombo<TextureFilterType>( "Texture Filter", Tex.TextureFilter ) );
            Parameters.Add( new UiCombo<TextureBorderType>( "Texture Border U", Tex.TextureBorderU ) );
            Parameters.Add( new UiCombo<TextureBorderType>( "Texture Border V", Tex.TextureBorderV ) );

            Tabs.Add( new UiCurve( Tex.NPow, "Power" ) );
        }

        public override void DrawUnassigned( string parentId ) {
            if( ImGui.SmallButton( "+ Texture Normal" + parentId ) ) Assign( Tex );
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/TN";
            DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => "Texture Normal";

        public override bool IsAssigned() => Tex.IsAssigned();

        public override List<UiNodeSelect> GetNodeSelects() => new() {
            new UiNodeSelect<UiTexture>( Particle, "Texture", Particle.NodeGroups.Textures, Tex.TextureIdx )
        };
    }
}
