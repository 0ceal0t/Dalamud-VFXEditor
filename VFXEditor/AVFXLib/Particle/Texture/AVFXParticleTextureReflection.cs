using System.Collections.Generic;
using System.IO;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Particle {
    public class AVFXParticleTextureReflection : AVFXBase {
        public readonly AVFXBool Enabled = new( "bEna" );
        public readonly AVFXBool UseScreenCopy = new( "bUSC" );
        public readonly AVFXEnum<TextureFilterType> TextureFilter = new( "TFT" );
        public readonly AVFXEnum<TextureCalculateColor> TextureCalculateColor = new( "TCCT" );
        public readonly AVFXInt TextureIdx = new( "TxNo" );
        public readonly AVFXCurve Rate = new( "Rate" );
        public readonly AVFXCurve RPow = new( "RPow" );

        private readonly List<AVFXBase> Children;

        public AVFXParticleTextureReflection() : base( "TR" ) {
            Children = new List<AVFXBase> {
                Enabled,
                UseScreenCopy,
                TextureFilter,
                TextureCalculateColor,
                TextureIdx,
                Rate,
                RPow
            };
            TextureIdx.SetValue( -1 );
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Children, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );
    }
}
