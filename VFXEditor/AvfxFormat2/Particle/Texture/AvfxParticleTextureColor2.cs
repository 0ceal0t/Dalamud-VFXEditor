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
    public class AvfxParticleTextureColor2 : AvfxParticleAttribute {
        public readonly string Name;

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

        private readonly List<AvfxBase> Children;

        public AvfxParticleTextureColor2( string name, string avfxName, AvfxParticle particle ) : base( avfxName, particle ) {
            Name = name;
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
                TextureIdx
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
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            ReadNested( reader, Children, size );
            EnableAllSelectors();
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );

        public override void DrawUnassigned( string parentId ) {
            if( ImGui.SmallButton( "+ " + Name + parentId ) ) Assign();
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/" + Name;

            if( AvfxName != "TC2" && UiUtils.RemoveButton( "Delete " + Name + id, small: true ) ) {
                Unassign();
                return;
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            IUiItem.DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => Name;

        public override List<UiNodeSelect> GetNodeSelects() => new() {
            new UiNodeSelect<AvfxTexture>( Particle, "Texture", Particle.NodeGroups.Textures, TextureIdx )
        };
    }
}
