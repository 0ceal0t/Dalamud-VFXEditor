using System.Collections.Generic;
using System.IO;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Particle {
    public class AVFXParticleTextureNormal : AVFXBase {
        public readonly AVFXBool Enabled = new( "bEna" );
        public readonly AVFXInt UvSetIdx = new( "UvSN" );
        public readonly AVFXEnum<TextureFilterType> TextureFilter = new( "TFT" );
        public readonly AVFXEnum<TextureBorderType> TextureBorderU = new( "TBUT" );
        public readonly AVFXEnum<TextureBorderType> TextureBorderV = new( "TBVT" );
        public readonly AVFXInt TextureIdx = new( "TxNo" );
        public readonly AVFXCurve NPow = new( "NPow" );

        private readonly List<AVFXBase> Children;

        public AVFXParticleTextureNormal() : base( "TN" ) {
            Children = new List<AVFXBase> {
                Enabled,
                UvSetIdx,
                TextureFilter,
                TextureBorderU,
                TextureBorderV,
                TextureIdx,
                NPow
            };
            TextureIdx.SetValue( -1 );
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Children, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );
    }
}
