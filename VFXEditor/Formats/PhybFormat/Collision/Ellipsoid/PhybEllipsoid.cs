using HelixToolkit.SharpDX.Animations;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.PhybFormat.Collision.Ellipsoid {
    public class PhybEllipsoid : PhybData, IPhysicsObject {
        public readonly ParsedPaddedString Name = new( "Name", "replace_me", 32, 0xFE );
        public readonly ParsedPaddedString Bone = new( "Bone", 32, 0xFE );
        public readonly ParsedFloat3 Offset1 = new( "Bone Offset 1" );
        public readonly ParsedFloat3 Offset2 = new( "Bone Offset 2" );
        public readonly ParsedFloat3 Offset3 = new( "Bone Offset 3" );
        public readonly ParsedFloat3 Offset4 = new( "Bone Offset 4" );
        public readonly ParsedFloat Radius = new( "Radius" );

        public PhybEllipsoid( PhybFile file ) : base( file ) { }

        public PhybEllipsoid( PhybFile file, BinaryReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Name,
            Bone,
            Offset1,
            Offset2,
            Offset3,
            Offset4,
            Radius,
        ];

        public void AddPhysicsObjects( MeshBuilders meshes, Dictionary<string, Bone> boneMatrixes ) {

        }
    }
}
