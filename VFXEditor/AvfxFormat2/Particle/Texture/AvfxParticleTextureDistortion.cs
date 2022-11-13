using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleTextureDistortion : AvfxParticleAttribute {
        public readonly AvfxBool Enabled = new( "Enabled", "bEna" );
        public readonly AvfxBool TargetUV1 = new( "Distort UV1", "bT1" );
        public readonly AvfxBool TargetUV2 = new( "Distort UV2", "bT2" );
        public readonly AvfxBool TargetUV3 = new( "Distort UV3", "bT3" );
        public readonly AvfxBool TargetUV4 = new( "Distort UV4", "bT4" );
        public readonly AvfxInt UvSetIdx = new( "UV Set Index", "UvSN" );
        public readonly AvfxEnum<TextureFilterType> TextureFilter = new( "Texture Filter", "TFT" );
        public readonly AvfxEnum<TextureBorderType> TextureBorderU = new( "Texture Border U", "TBUT" );
        public readonly AvfxEnum<TextureBorderType> TextureBorderV = new( "Texture Border V", "TBVT" );
        public readonly AvfxInt TextureIdx = new( "Texture Index", "TxNo" );
        public readonly AvfxCurve DPow = new( "Power", "DPow" );

        private readonly List<AvfxBase> Children;

        public AvfxParticleTextureDistortion( AvfxParticle particle ) : base( "TD", particle ) {
            InitNodeSelects();

            Children = new() {
                Enabled,
                TargetUV1,
                TargetUV2,
                TargetUV3,
                TargetUV4,
                UvSetIdx,
                TextureFilter,
                TextureBorderU,
                TextureBorderV,
                TextureIdx,
                DPow
            };
            TextureIdx.SetValue( -1 );

            Parameters.Add( Enabled );
            Parameters.Add( TargetUV1 );
            Parameters.Add( TargetUV2 );
            Parameters.Add( TargetUV3 );
            Parameters.Add( TargetUV4 );
            Parameters.Add( UvSetIdx );
            Parameters.Add( TextureFilter );
            Parameters.Add( TextureBorderU );
            Parameters.Add( TextureBorderV );

            Tabs.Add( DPow );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            ReadNested( reader, Children, size );
            EnableAllSelectors();
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );

        public override void DrawUnassigned( string parentId ) {
            if( ImGui.SmallButton( "+ Texture Distortion" + parentId ) ) Assign();
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/TD";
            IUiItem.DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => "Texture Distortion";

        public override List<UiNodeSelect> GetNodeSelects() => new() {
            new UiNodeSelect<AvfxTexture>( Particle, "Texture", Particle.NodeGroups.Textures, TextureIdx )
        };
    }
}
