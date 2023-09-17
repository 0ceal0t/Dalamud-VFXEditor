using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.AvfxFormat.Particle.Texture;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleTextureNormal : AvfxParticleAttribute {
        public readonly AvfxBool Enabled = new( "Enabled", "bEna" );
        public readonly AvfxInt UvSetIdx = new( "UV Set Index", "UvSN" );
        public readonly AvfxEnum<TextureFilterType> TextureFilter = new( "Texture Filter", "TFT" );
        public readonly AvfxEnum<TextureBorderType> TextureBorderU = new( "Texture Border U", "TBUT" );
        public readonly AvfxEnum<TextureBorderType> TextureBorderV = new( "Texture Border V", "TBVT" );
        public readonly AvfxInt TextureIdx = new( "Texture Index", "TxNo", value: -1 );
        public readonly AvfxCurve NPow = new( "Power", "NPow" );

        private readonly List<AvfxBase> Parsed;

        public AvfxParticleTextureNormal( AvfxParticle particle ) : base( "TN", particle ) {
            InitNodeSelects();
            Display.Add( new TextureNodeSelectDraw( NodeSelects ) );

            Parsed = [
                Enabled,
                UvSetIdx,
                TextureFilter,
                TextureBorderU,
                TextureBorderV,
                TextureIdx,
                NPow
            ];

            Display.Add( Enabled );
            Display.Add( UvSetIdx );
            Display.Add( TextureFilter );
            Display.Add( TextureBorderU );
            Display.Add( TextureBorderV );

            DisplayTabs.Add( NPow );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            ReadNested( reader, Parsed, size );
            EnableAllSelectors();
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Parsed, assigned );

        public override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Parsed );

        public override void DrawUnassigned() {
            using var _ = ImRaii.PushId( "TN" );

            AssignedCopyPaste( this, GetDefaultText() );
            if( ImGui.SmallButton( "+ Texture Normal" ) ) Assign();
        }

        public override void DrawAssigned() {
            using var _ = ImRaii.PushId( "TN" );

            AssignedCopyPaste( this, GetDefaultText() );
            DrawNamedItems( DisplayTabs );
        }

        public override string GetDefaultText() => "Texture Normal";

        public override List<UiNodeSelect> GetNodeSelects() => new() {
            new UiNodeSelect<AvfxTexture>( Particle, "Texture", Particle.NodeGroups.Textures, TextureIdx )
        };
    }
}
