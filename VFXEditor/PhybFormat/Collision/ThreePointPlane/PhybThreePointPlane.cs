using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.PhybFormat.Collision.ThreePointPlane {
    public class PhybThreePointPlane : PhybPhysicsData, IPhysicsObject {
        public readonly ParsedPaddedString Name = new( "Name", 32, 0xFE );
        public readonly ParsedPaddedString Bone = new( "Bone", 32, 0xFE );
        public readonly ParsedReserve Padding1 = new( 64 );
        public readonly ParsedFloat3 BoneOffset = new( "Bone Offset" );
        public readonly ParsedReserve Padding2 = new( 24 );
        public readonly ParsedFloat Thickness = new( "Thickness" );

        public PhybThreePointPlane( PhybFile file ) : base( file ) { }

        public PhybThreePointPlane( PhybFile file, BinaryReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => new() {
            Name,
            Bone,
            Padding1,
            BoneOffset,
            Padding2,
            Thickness,
        };

        public void AddPhysicsObjects( MeshBuilder builder, Dictionary<string, Bone> boneMatrixes ) {

        }
    }
}
