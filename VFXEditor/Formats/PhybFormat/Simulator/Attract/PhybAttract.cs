using HelixToolkit.SharpDX.Animations;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.PhybFormat.Simulator.Attract {
    public class PhybAttract : PhybData, IPhysicsObject {
        public readonly PhybSimulator Simulator;

        public readonly ParsedPaddedString BoneName = new( "Bone Name", 32, 0xFE );
        public readonly ParsedFloat3 BoneOffset = new( "Bone Offset" );
        public readonly ParsedShort ChainId = new( "Chain Id" );
        public readonly ParsedShort NodeId = new( "Node Id" );
        public readonly ParsedFloat Stiffness = new( "Stiffness" );

        public PhybAttract( PhybFile file, PhybSimulator simulator ) : base( file ) {
            Simulator = simulator;
        }

        public PhybAttract( PhybFile file, PhybSimulator simulator, BinaryReader reader ) : base( file, reader ) {
            Simulator = simulator;
        }

        protected override List<ParsedBase> GetParsed() => [
            BoneName,
            BoneOffset,
            ChainId,
            NodeId,
            Stiffness,
        ];

        public void AddPhysicsObjects( MeshBuilders meshes, Dictionary<string, Bone> boneMatrixes ) {
            Simulator.ConnectNodeToBone( ChainId.Value, NodeId.Value, BoneName.Value, meshes.Spring, boneMatrixes );
        }
    }
}
