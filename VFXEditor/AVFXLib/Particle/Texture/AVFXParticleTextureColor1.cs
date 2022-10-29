using System.Collections.Generic;
using System.IO;
using VfxEditor.AVFXLib.Curve;

namespace VfxEditor.AVFXLib.Particle {
    public class AVFXParticleTextureColor1 : AVFXBase {
        public readonly AVFXBool Enabled = new( "bEna" );
        public readonly AVFXBool ColorToAlpha = new( "bC2A" );
        public readonly AVFXBool UseScreenCopy = new( "bUSC" );
        public readonly AVFXBool PreviousFrameCopy = new( "bPFC" );
        public readonly AVFXInt UvSetIdx = new( "UvSN" );
        public readonly AVFXEnum<TextureFilterType> TextureFilter = new( "TFT" );
        public readonly AVFXEnum<TextureBorderType> TextureBorderU = new( "TBUT" );
        public readonly AVFXEnum<TextureBorderType> TextureBorderV = new( "TBVT" );
        public readonly AVFXEnum<TextureCalculateColor> TextureCalculateColor = new( "TCCT" );
        public readonly AVFXEnum<TextureCalculateAlpha> TextureCalculateAlpha = new( "TCAT" );
        public readonly AVFXInt TextureIdx = new( "TxNo" );
        public readonly AVFXIntList MaskTextureIdx = new( "TLst" );
        public readonly AVFXCurve TexN = new( "TxN" );
        public readonly AVFXCurve TexNRandom = new( "TxNR" );

        private readonly List<AVFXBase> Children;

        public AVFXParticleTextureColor1() : base( "TC1" ) {
            Children = new List<AVFXBase> {
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
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Children, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );
    }
}
