using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleTextureNormal : AvfxParticleAttribute {
        public readonly AvfxBool Enabled = new( "Enabled", "bEna" );
        public readonly AvfxInt UvSetIdx = new( "UV Set Index", "UvSN" );
        public readonly AvfxEnum<TextureFilterType> TextureFilter = new( "Texture Filter", "TFT" );
        public readonly AvfxEnum<TextureBorderType> TextureBorderU = new( "Texture Border U", "TBUT" );
        public readonly AvfxEnum<TextureBorderType> TextureBorderV = new( "Texture Border V", "TBVT" );
        public readonly AvfxInt TextureIdx = new( "Texture Index", "TxNo" );
        public readonly AvfxCurve NPow = new( "Power", "NPow" );

        private readonly List<AvfxBase> Children;

        public AvfxParticleTextureNormal( AvfxParticle particle ) : base( "TN", particle ) {
            InitNodeSelects();

            Children = new() {
                Enabled,
                UvSetIdx,
                TextureFilter,
                TextureBorderU,
                TextureBorderV,
                TextureIdx,
                NPow
            };
            TextureIdx.SetValue( -1 );

            Parameters.Add( Enabled );
            Parameters.Add( UvSetIdx );
            Parameters.Add( TextureFilter );
            Parameters.Add( TextureBorderU );
            Parameters.Add( TextureBorderV );

            Tabs.Add( NPow );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            ReadNested( reader, Children, size );
            EnableAllSelectors();
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );

        public override void DrawUnassigned( string parentId ) {
            if( ImGui.SmallButton( "+ Texture Normal" + parentId ) ) Assign();
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/TN";
            IUiItem.DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => "Texture Normal";

        public override List<UiNodeSelect> GetNodeSelects() => new() {
            new UiNodeSelect<AvfxTexture>( Particle, "Texture", Particle.NodeGroups.Textures, TextureIdx )
        };
    }
}
