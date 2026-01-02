using HelixToolkit.SharpDX.Animations;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.PhybFormat.Collision.ThreePointPlane {
    public class PhybThreePointPlane : PhybData, IPhysicsObject {
        public readonly ParsedPaddedString Name = new( "Name", "replace_me", 32, 0xFE );
        public readonly ParsedPaddedString Bone = new( "Bone", 32, 0xFE );
        public readonly ParsedFloat4 Unknown1 = new( "Unknown 1" );
        public readonly ParsedFloat4 Unknown2 = new( "Unknown 2" );
        public readonly ParsedFloat4 Unknown3 = new( "Unknown 3" );
        public readonly ParsedFloat4 Unknown4 = new( "Unknown 4" );
        public readonly ParsedFloat3 BoneOffset = new( "Bone Offset" );
        public readonly ParsedFloat3 Unknown5 = new( "Unknown 5" );
        public readonly ParsedFloat3 Unknown6 = new( "Unknown 6" );
        public readonly ParsedFloat Thickness = new( "Thickness" );

        public PhybThreePointPlane( PhybFile file ) : base( file ) { }

        public PhybThreePointPlane( PhybFile file, BinaryReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Name,
            Bone,
            Unknown1,
            Unknown2,
            Unknown3,
            Unknown4,
            BoneOffset,
            Unknown5,
            Unknown6,
            Thickness,
        ];

        public void AddPhysicsObjects( MeshBuilders meshes, Dictionary<string, Bone> boneMatrixes ) {

        }
    }
}
