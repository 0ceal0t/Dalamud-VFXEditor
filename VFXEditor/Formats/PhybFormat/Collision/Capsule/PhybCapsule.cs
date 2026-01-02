using HelixToolkit.Maths;
using HelixToolkit.SharpDX.Animations;
using HelixToolkit.SharpDX.Core;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.PhybFormat.Collision.Capsule {
    public class PhybCapsule : PhybData, IPhysicsObject {
        public readonly ParsedPaddedString Name = new( "Name", "replace_me", 32, 0xFE );
        public readonly ParsedPaddedString StartBone = new( "Start Bone", 32, 0xFE );
        public readonly ParsedPaddedString EndBone = new( "End Bone", 32, 0xFE );
        public readonly ParsedFloat3 StartOffset = new( "Start Offset" );
        public readonly ParsedFloat3 EndOffset = new( "End Offset" );
        public readonly ParsedFloat Radius = new( "Radius" );

        public PhybCapsule( PhybFile file ) : base( file ) { }

        public PhybCapsule( PhybFile file, BinaryReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Name,
            StartBone,
            EndBone,
            StartOffset,
            EndOffset,
            Radius,
        ];

        public void AddPhysicsObjects( MeshBuilders meshes, Dictionary<string, Bone> boneMatrixes ) {
            if( !boneMatrixes.TryGetValue( StartBone.Value, out var startBone ) ) return;
            if( !boneMatrixes.TryGetValue( EndBone.Value, out var endBone ) ) return;

            var startOffset = new Vector3( StartOffset.Value.X, StartOffset.Value.Y, StartOffset.Value.Z );
            var endOffset = new Vector3( EndOffset.Value.X, EndOffset.Value.Y, EndOffset.Value.Z );

            var startPos = Vector3Helper.TransformCoordinate( startOffset, startBone.BindPose );
            var endPos = Vector3Helper.TransformCoordinate( endOffset, endBone.BindPose );

            meshes.Collision.AddCylinder( startPos, endPos, Radius.Value * 2f, 10 );
            meshes.Collision.AddSphere( startPos, Radius.Value, 10, 10 );
            meshes.Collision.AddSphere( endPos, Radius.Value, 10, 10 );
        }
    }
}
