using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using SharpDX;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.PhybFormat.Simulator.Pin {
    public class PhybPin : PhybPhysicsData, IPhysicsObject {
        public readonly PhybSimulator Simulator;

        public readonly ParsedPaddedString BoneName = new( "Bone Name", 32, 0xFE );
        public readonly ParsedFloat3 BoneOffset = new( "Bone Offset" );
        public readonly ParsedShort ChainId = new( "Chain Id" );
        public readonly ParsedShort NodeId = new( "Node Id" );

        public PhybPin( PhybFile file, PhybSimulator simulator ) : base( file ) {
            Simulator = simulator;
        }

        public PhybPin( PhybFile file, PhybSimulator simulator, BinaryReader reader ) : base( file, reader ) {
            Simulator = simulator;
        }

        protected override List<ParsedBase> GetParsed() => new() {
            BoneName,
            BoneOffset,
            ChainId,
            NodeId,
        };

        public void AddPhysicsObjects( MeshBuilders meshes, Dictionary<string, Bone> boneMatrixes ) {
            if( !Simulator.GetBone( ChainId.Value, NodeId.Value, boneMatrixes, out var bone1 ) ) return;
            if( !boneMatrixes.TryGetValue( BoneName.Value, out var bone2 ) ) return;

            var pos1 = bone1.BindPose.TranslationVector;

            var offset = new Vector3( BoneOffset.Value.X, BoneOffset.Value.Y, BoneOffset.Value.Z );
            var pos2 = Vector3.Transform( offset, bone2.BindPose ).ToVector3();

            meshes.Spring.AddCylinder( pos1, pos2, 0.02f, 5 );
            meshes.Spring.AddSphere( pos2, 0.03f, 10, 10 );
        }
    }
}
