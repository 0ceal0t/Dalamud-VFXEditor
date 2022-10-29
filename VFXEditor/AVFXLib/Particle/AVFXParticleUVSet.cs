using System.Collections.Generic;
using System.IO;
using VfxEditor.AVFXLib.Curve;

namespace VfxEditor.AVFXLib.Particle {
    public class AVFXParticleUVSet : AVFXBase {
        public readonly AVFXEnum<TextureCalculateUV> CalculateUVType = new( "CUvT" );
        public readonly AVFXCurve2Axis Scale = new( "Scl" );
        public readonly AVFXCurve2Axis Scroll = new( "Scr" );
        public readonly AVFXCurve Rot = new( "Rot" );
        public readonly AVFXCurve RotRandom = new( "RotR" );

        private readonly List<AVFXBase> Children;

        public AVFXParticleUVSet() : base( "UvSt" ) {
            Children = new List<AVFXBase> {
                CalculateUVType,
                Scale,
                Scroll,
                Rot,
                RotRandom
            };
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Children, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );
    }
}
