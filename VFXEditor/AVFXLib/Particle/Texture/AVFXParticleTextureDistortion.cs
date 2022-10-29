using System.Collections.Generic;
using System.IO;
using VfxEditor.AVFXLib.Curve;

namespace VfxEditor.AVFXLib.Particle {
    public class AVFXParticleTextureDistortion : AVFXBase {
        public readonly AVFXBool Enabled = new( "bEna" );
        public readonly AVFXBool TargetUV1 = new( "bT1" );
        public readonly AVFXBool TargetUV2 = new( "bT2" );
        public readonly AVFXBool TargetUV3 = new( "bT3" );
        public readonly AVFXBool TargetUV4 = new( "bT4" );
        public readonly AVFXInt UvSetIdx = new( "UvSN" );
        public readonly AVFXEnum<TextureFilterType> TextureFilter = new( "TFT" );
        public readonly AVFXEnum<TextureBorderType> TextureBorderU = new( "TBUT" );
        public readonly AVFXEnum<TextureBorderType> TextureBorderV = new( "TBVT" );
        public readonly AVFXInt TextureIdx = new( "TxNo" );
        public readonly AVFXCurve DPow = new( "DPow" );

        private readonly List<AVFXBase> Children;

        public AVFXParticleTextureDistortion() : base( "TD" ) {
            Children = new List<AVFXBase> {
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
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Children, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );
    }
}
