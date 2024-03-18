using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.AvfxFormat.Particle.Texture;
using VfxEditor.Utils;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
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
        public readonly AvfxInt TextureIdx = new( "Texture Index", "TxNo", value: -1 );

        private readonly List<AvfxBase> Parsed;

        public AvfxParticleTextureColor2( string name, string avfxName, AvfxParticle particle ) : base( avfxName, particle ) {
            Name = name;
            InitNodeSelects();
            Display.Add( new TextureNodeSelectDraw( NodeSelects ) );

            Parsed = [
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
            ];

            Display.Add( Enabled );
            Display.Add( ColorToAlpha );
            Display.Add( UseScreenCopy );
            Display.Add( PreviousFrameCopy );
            Display.Add( UvSetIdx );
            Display.Add( TextureFilter );
            Display.Add( TextureBorderU );
            Display.Add( TextureBorderV );
            Display.Add( TextureCalculateColor );
            Display.Add( TextureCalculateAlpha );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            ReadNested( reader, Parsed, size );
            EnableAllSelectors();
        }

        public override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Parsed );

        protected override IEnumerable<AvfxBase> GetChildren() {
            foreach( var item in Parsed ) yield return item;
        }

        public override void DrawUnassigned() {
            using var _ = ImRaii.PushId( Name );

            AssignedCopyPaste( Name );
            if( ImGui.SmallButton( $"+ {Name}" ) ) Assign();
        }

        public override void DrawAssigned() {
            using var _ = ImRaii.PushId( Name );

            AssignedCopyPaste( Name );
            if( AvfxName != "TC2" && UiUtils.RemoveButton( $"Delete {Name}", small: true ) ) {
                Unassign();
                return;
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DrawNamedItems( DisplayTabs );
        }

        public override string GetDefaultText() => Name;

        public override List<AvfxNodeSelect> GetNodeSelects() => [
            new AvfxNodeSelect<AvfxTexture>( Particle, "Texture", Particle.NodeGroups.Textures, TextureIdx )
        ];
    }
}
