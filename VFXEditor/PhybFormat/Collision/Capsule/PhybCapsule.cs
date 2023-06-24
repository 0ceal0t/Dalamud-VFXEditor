using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using SharpDX;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.PhybFormat.Collision.Capsule {
    public class PhybCapsule : PhybPhysicsData, IPhysicsObject {
        public readonly ParsedPaddedString Name = new( "Name", 32, 0xFE );
        public readonly ParsedPaddedString StartBone = new( "Start Bone", 32, 0xFE );
        public readonly ParsedPaddedString EndBone = new( "End Bone", 32, 0xFE );
        public readonly ParsedFloat3 StartOffset = new( "Start Offset" );
        public readonly ParsedFloat3 EndOffset = new( "End Offset" );
        public readonly ParsedFloat Radius = new( "Radius" );

        public PhybCapsule( PhybFile file ) : base( file ) { }

        public PhybCapsule( PhybFile file, BinaryReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => new() {
            Name,
            StartBone,
            EndBone,
            StartOffset,
            EndOffset,
            Radius,
        };

        public void AddPhysicsObjects( MeshBuilder builder, Dictionary<string, Bone> boneMatrixes ) {
            if( !boneMatrixes.TryGetValue( StartBone.Value, out var startBone ) ) return;
            if( !boneMatrixes.TryGetValue( EndBone.Value, out var endBone ) ) return;

            var startOffset = new Vector3( StartOffset.Value.X, StartOffset.Value.Y, StartOffset.Value.Z );
            var endOffset = new Vector3( EndOffset.Value.X, EndOffset.Value.Y, EndOffset.Value.Z );

            var startPos = Vector3.Transform( startOffset, startBone.BindPose );
            var endPos = Vector3.Transform( endOffset, endBone.BindPose );

            var p1 = new Vector3( startPos.X, startPos.Y, startPos.Z );
            var p2 = new Vector3( endPos.X, endPos.Y, endPos.Z );

            builder.AddCylinder( p1, p2, Radius.Value * 2f, 10 );
            builder.AddSphere( p1, Radius.Value, 10, 10 );
            builder.AddSphere( p2, Radius.Value, 10, 10 );
        }
    }
}
