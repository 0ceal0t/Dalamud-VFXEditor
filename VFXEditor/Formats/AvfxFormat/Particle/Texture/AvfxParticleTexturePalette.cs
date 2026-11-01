using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.AvfxFormat.Base;
using VfxEditor.Formats.AvfxFormat.Components.Bool;
using VfxEditor.Formats.AvfxFormat.Components.Enums;
using VfxEditor.Formats.AvfxFormat.Components.Int;
using VfxEditor.Formats.AvfxFormat.Curve;
using VfxEditor.Formats.AvfxFormat.Nodes.Select;
using VfxEditor.Formats.AvfxFormat.Texture;
using static VfxEditor.Formats.AvfxFormat.Base.Enums;

namespace VfxEditor.Formats.AvfxFormat.Particle.Texture {
    public class AvfxParticleTexturePalette : AvfxParticleAttribute {
        public readonly AvfxBool Enabled = new( "Enabled", "bEna" );
        public readonly AvfxEnum<TextureFilterType> TextureFilter = new( "Texture Filter", "TFT" );
        public readonly AvfxEnum<TextureBorderType> TextureBorder = new( "Texture Border", "TBT" );
        public readonly AvfxInt TextureIdx = new( "Texture Index", "TxNo", value: -1 );
        public readonly AvfxCurve1Axis Offset = new( "Offset", "POff" );
        public readonly AvfxCurve1Axis OffsetRandom = new( "Offset Random", "POfR" );

        private readonly List<AvfxBase> Parsed;

        public AvfxParticleTexturePalette( AvfxParticle particle ) : base( "TP", particle, locked: true ) {
            InitNodeSelects();
            Display.Add( new TextureNodeSelectDraw( NodeSelects ) );

            Parsed = GetFieldsInOrder<AvfxBase>( typeof( AvfxParticleTexturePalette ) );

            Display.Add( Enabled );
            Display.Add( TextureFilter );
            Display.Add( TextureBorder );

            DisplayTabs.Add( Offset );
            DisplayTabs.Add( OffsetRandom );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            ReadNested( reader, Parsed, size );
            EnableAllSelectors();
        }

        public override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Parsed );

        protected override IEnumerable<AvfxBase> GetChildren() {
            foreach( var item in Parsed ) yield return item;
        }

        public override void DrawBody() {
            DrawNamedItems( DisplayTabs );
        }

        public override string GetDefaultText() => "Texture Palette";

        public override List<AvfxNodeSelect> GetNodeSelects() => [
            new AvfxNodeSelect<AvfxTexture>( Particle, "Texture", Particle.NodeGroups.Textures, TextureIdx )
        ];
    }
}
