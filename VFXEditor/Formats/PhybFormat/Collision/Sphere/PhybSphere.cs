using HelixToolkit.Maths;
using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Animations;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.PhybFormat.Collision.Sphere {
    public class PhybSphere : PhybData, IPhysicsObject {
        public readonly ParsedPaddedString Name = new( "Name", "replace_me", 32, 0xFE );
        public readonly ParsedPaddedString Bone = new( "Bone", 32, 0xFE );
        public readonly ParsedFloat3 BoneOffset = new( "Bone Offset" );
        public readonly ParsedFloat Thickness = new( "Thickness" );

        public PhybSphere( PhybFile file ) : base( file ) { }

        public PhybSphere( PhybFile file, BinaryReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Name,
            Bone,
            BoneOffset,
            Thickness,
        ];

        public void AddPhysicsObjects( MeshBuilders meshes, Dictionary<string, Bone> boneMatrixes ) {
            if( !boneMatrixes.TryGetValue( Bone.Value, out var bone ) ) return;
            var offset = new Vector3( BoneOffset.Value.X, BoneOffset.Value.Y, BoneOffset.Value.Z );
            var pos = Vector3Helper.TransformCoordinate( offset, bone.BindPose );
            meshes.Collision.AddSphere( pos, Thickness.Value / 2f, 10, 10 );
        }
    }
}
