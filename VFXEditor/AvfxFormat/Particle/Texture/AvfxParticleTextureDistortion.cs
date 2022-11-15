using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
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

        private readonly List<AvfxBase> Parsed;

        public AvfxParticleTextureDistortion( AvfxParticle particle ) : base( "TD", particle ) {
            InitNodeSelects();

            Parsed = new() {
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

            Display.Add( Enabled );
            Display.Add( TargetUV1 );
            Display.Add( TargetUV2 );
            Display.Add( TargetUV3 );
            Display.Add( TargetUV4 );
            Display.Add( UvSetIdx );
            Display.Add( TextureFilter );
            Display.Add( TextureBorderU );
            Display.Add( TextureBorderV );

            DisplayTabs.Add( DPow );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            ReadNested( reader, Parsed, size );
            EnableAllSelectors();
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Parsed, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Parsed );

        public override void DrawUnassigned( string parentId ) {
            AssignedCopyPaste( this, GetDefaultText() );
            if( ImGui.SmallButton( "+ Texture Distortion" + parentId ) ) Assign();
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/TD";

            AssignedCopyPaste( this, GetDefaultText() );
            IUiItem.DrawListTabs( DisplayTabs, id );
        }

        public override string GetDefaultText() => "Texture Distortion";

        public override List<UiNodeSelect> GetNodeSelects() => new() {
            new UiNodeSelect<AvfxTexture>( Particle, "Texture", Particle.NodeGroups.Textures, TextureIdx )
        };
    }
}
