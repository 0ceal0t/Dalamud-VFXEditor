using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTexturePalette : UiParticleAttribute {
        public readonly AVFXParticleTexturePalette Tex;
        public readonly string Name;

        public UiTexturePalette( AVFXParticleTexturePalette tex, UiParticle particle ) : base( particle ) {
            Tex = tex;
            InitNodeSelects();

            Parameters.Add( new UiCheckbox( "Enabled", Tex.Enabled ) );
            Parameters.Add( new UiCombo<TextureFilterType>( "Texture Filter", Tex.TextureFilter ) );
            Parameters.Add( new UiCombo<TextureBorderType>( "Texture Border", Tex.TextureBorder ) );

            Tabs.Add( new UiCurve( Tex.Offset, "Offset" ) );
        }

        public override void DrawUnassigned( string parentId ) {
            if( ImGui.SmallButton( "+ Texture Palette" + parentId ) ) Assign( Tex );
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/TP";
            DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => "Texture Palette";

        public override bool IsAssigned() => Tex.IsAssigned();

        public override List<UiNodeSelect> GetNodeSelects() => new() {
            new UiNodeSelect<UiTexture>(Particle, "Texture", Particle.NodeGroups.Textures, Tex.TextureIdx )
        };
    }
}
