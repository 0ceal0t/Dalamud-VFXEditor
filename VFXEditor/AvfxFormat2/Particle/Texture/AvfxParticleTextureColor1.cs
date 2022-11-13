using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Utils;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleTextureColor1 : AvfxParticleAttribute {
        public readonly AvfxBool Enabled = new( "Enabled", "bEna" );
        public readonly AvfxBool ColorToAlpha = new( "Color To Alpha", "bC2A" );
        public readonly AvfxBool UseScreenCopy = new( "Use Screen Copy", "bUSC" );
        public readonly AvfxBool PreviousFrameCopy = new( "Previous Frame Copy", "bPFC" );
        public readonly AvfxInt UvSetIdx = new( "UV Set Index", "UvSN" );
        public readonly AvfxEnum<TextureFilterType> TextureFilter = new( "Texture Filter", "TFT" );
        public readonly AvfxEnum<TextureBorderType> TextureBorderU = new( "Texture Border U", "TBUT" );
        public readonly AvfxEnum<TextureBorderType> TextureBorderV = new( "Texture Border V", "TBVT" );
        public readonly AvfxEnum<TextureCalculateColor> TextureCalculateColor = new( "Calculate Color", "TCCT" );
        public readonly AvfxEnum<TextureCalculateAlpha> TextureCalculateAlpha = new( "Calculate Alpha", "TCAT" );
        public readonly AvfxInt TextureIdx = new( "Texture Index", "TxNo" );
        public readonly AvfxIntList MaskTextureIdx = new( "Mask Index", "TLst" );
        public readonly AvfxCurve TexN = new( "TexN", "TxN" );
        public readonly AvfxCurve TexNRandom = new( "TexN Random", "TxNR" );

        private readonly List<AvfxBase> Children;

        public AvfxParticleTextureColor1( AvfxParticle particle ) : base( "TC1", particle ) {
            InitNodeSelects();

            Children = new() {
                Enabled,
                ColorToAlpha,
                UseScreenCopy,
                PreviousFrameCopy,
                UvSetIdx,
                TextureFilter,
                TextureBorderU,
                TextureBorderV,
                TextureCalculateColor,
                TextureCalculateAlpha,
                TextureIdx,
                MaskTextureIdx,
                TexN,
                TexNRandom
            };
            TextureIdx.SetValue( -1 );

            Parameters.Add( Enabled );
            Parameters.Add( ColorToAlpha );
            Parameters.Add( UseScreenCopy );
            Parameters.Add( PreviousFrameCopy );
            Parameters.Add( UvSetIdx );
            Parameters.Add( TextureFilter );
            Parameters.Add( TextureBorderU );
            Parameters.Add( TextureBorderV );
            Parameters.Add( TextureCalculateColor );
            Parameters.Add( TextureCalculateAlpha );
            Tabs.Add( TexN );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            ReadNested( reader, Children, size );
            EnableAllSelectors();
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );

        public override void DrawUnassigned( string parentId ) {
            if( ImGui.SmallButton( "+ Texture Color 1" + parentId ) ) Assign();
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/TC1";

            if( UiUtils.RemoveButton( "Delete Texture Color 1" + id, small: true ) ) {
                Unassign();
                return;
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            IUiItem.DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => "Texture Color 1";

        public override List<UiNodeSelect> GetNodeSelects() => new() {
            new UiNodeSelectList<AvfxTexture>( Particle, "Mask Texture", Particle.NodeGroups.Textures, MaskTextureIdx )
        };
    }
}
