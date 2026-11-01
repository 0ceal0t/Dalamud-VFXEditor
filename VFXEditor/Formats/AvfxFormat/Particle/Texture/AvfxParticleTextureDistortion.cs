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
    public class AvfxParticleTextureDistortion : AvfxParticleAttribute {
        public readonly AvfxBool Enabled = new( "Enabled", "bEna" );
        public readonly AvfxBool TargetUv1 = new( "Distort UV 1", "bT1" );
        public readonly AvfxBool TargetUv2 = new( "Distort UV 2", "bT2" );
        public readonly AvfxBool TargetUv3 = new( "Distort UV 3", "bT3" );
        public readonly AvfxBool TargetUv4 = new( "Distort UV 4", "bT4" );
        public readonly AvfxInt UvSetIdx = new( "UV Set Index", "UvSN" );
        public readonly AvfxEnum<TextureFilterType> TextureFilter = new( "Texture Filter", "TFT" );
        public readonly AvfxEnum<TextureBorderType> TextureBorderU = new( "Texture Border U", "TBUT" );
        public readonly AvfxEnum<TextureBorderType> TextureBorderV = new( "Texture Border V", "TBVT" );
        public readonly AvfxInt TextureIdx = new( "Texture Index", "TxNo", value: -1 );
        public readonly AvfxCurve1Axis DPow = new( "Power", "DPow" );

        private readonly List<AvfxBase> Parsed;

        public AvfxParticleTextureDistortion( AvfxParticle particle ) : base( "TD", particle, locked: true ) {
            InitNodeSelects();
            Display.Add( new TextureNodeSelectDraw( NodeSelects ) );

            Parsed = GetFieldsInOrder<AvfxBase>( typeof( AvfxParticleTextureDistortion ) );

            Display.Add( Enabled );
            Display.Add( TargetUv1 );
            Display.Add( TargetUv2 );
            Display.Add( TargetUv3 );
            Display.Add( TargetUv4 );
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

        public override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Parsed );

        protected override IEnumerable<AvfxBase> GetChildren() {
            foreach( var item in Parsed ) yield return item;
        }

        public override void DrawBody() {
            DrawNamedItems( DisplayTabs );
        }

        public override string GetDefaultText() => "Texture Distortion";

        public override List<AvfxNodeSelect> GetNodeSelects() => [
            new AvfxNodeSelect<AvfxTexture>( Particle, "Texture", Particle.NodeGroups.Textures, TextureIdx )
        ];
    }
}
