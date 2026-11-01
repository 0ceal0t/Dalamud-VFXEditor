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
    public class AvfxParticleTextureNormal : AvfxParticleAttribute {
        public readonly AvfxBool Enabled = new( "Enabled", "bEna" );
        public readonly AvfxInt UvSetIdx = new( "UV Set Index", "UvSN" );
        public readonly AvfxEnum<TextureFilterType> TextureFilter = new( "Texture Filter", "TFT" );
        public readonly AvfxEnum<TextureBorderType> TextureBorderU = new( "Texture Border U", "TBUT" );
        public readonly AvfxEnum<TextureBorderType> TextureBorderV = new( "Texture Border V", "TBVT" );
        public readonly AvfxInt TextureIdx = new( "Texture Index", "TxNo", value: -1 );
        public readonly AvfxCurve1Axis NPow = new( "Power", "NPow" );

        private readonly List<AvfxBase> Parsed;

        public AvfxParticleTextureNormal( AvfxParticle particle ) : base( "TN", particle, locked: true ) {
            InitNodeSelects();
            Display.Add( new TextureNodeSelectDraw( NodeSelects ) );

            Parsed = GetFieldsInOrder<AvfxBase>( typeof( AvfxParticleTextureNormal ) );

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

        public override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Parsed );

        protected override IEnumerable<AvfxBase> GetChildren() {
            foreach( var item in Parsed ) yield return item;
        }

        public override void DrawBody() {
            DrawNamedItems( DisplayTabs );
        }

        public override string GetDefaultText() => "Texture Normal";

        public override List<AvfxNodeSelect> GetNodeSelects() => [
            new AvfxNodeSelect<AvfxTexture>( Particle, "Texture", Particle.NodeGroups.Textures, TextureIdx )
        ];
    }
}
