using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AVFXLib.Particle {
    public class AVFXParticleTextureColor2 : AVFXBase {
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

        private readonly List<AVFXBase> Children;

        public AVFXParticleTextureColor2( string name ) : base( name ) {
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
                TextureIdx
            };
            TextureIdx.SetValue( -1 );
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Children, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );
    }
}
