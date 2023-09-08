using HelixToolkit.SharpDX.Core.Animations;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.PhybFormat.Simulator.Connector {
    public class PhybConnector : PhybData, IPhysicsObject {
        public readonly PhybSimulator Simulator;

        public readonly ParsedShort ChainId1 = new( "Chain Id 1" );
        public readonly ParsedShort ChainId2 = new( "Chain Id 2" );
        public readonly ParsedShort NodeId1 = new( "Node Id 1" );
        public readonly ParsedShort NodeId2 = new( "Node Id 2" );
        public readonly ParsedFloat CollisionRadius = new( "Collision Radius" );
        public readonly ParsedFloat Friction = new( "Friction" );
        public readonly ParsedFloat Dampening = new( "Dampening" );
        public readonly ParsedFloat Repulsion = new( "Repulsion" );
        public readonly ParsedUInt CollisionFlag = new( "Collision Flags" );
        public readonly ParsedUInt ContinuousCollisionFlag = new( "Continuous Collision Flags" );

        public PhybConnector( PhybFile file, PhybSimulator simulator ) : base( file ) {
            Simulator = simulator;
        }

        public PhybConnector( PhybFile file, PhybSimulator simulator, BinaryReader reader ) : base( file, reader ) {
            Simulator = simulator;
        }

        protected override List<ParsedBase> GetParsed() => new() {
            ChainId1,
            ChainId2,
            NodeId1,
            NodeId2,
            CollisionRadius,
            Friction,
            Dampening,
            Repulsion,
            CollisionFlag,
            ContinuousCollisionFlag,
        };

        public void AddPhysicsObjects( MeshBuilders meshes, Dictionary<string, Bone> boneMatrixes ) {
            Simulator.ConnectNodes( ChainId1.Value, ChainId2.Value, NodeId1.Value, NodeId2.Value, CollisionRadius.Value, meshes.Simulation, boneMatrixes );
        }
    }
}
