using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.PhybFormat.Simulator.Connector {
    public class PhybConnector : PhybPhysicsData, IPhysicsObject {
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

        public void AddPhysicsObjects( MeshBuilder collision, MeshBuilder simulation, Dictionary<string, Bone> boneMatrixes ) {
            if( ChainId1.Value >= Simulator.Chains.Count ) return;
            if( ChainId2.Value >= Simulator.Chains.Count ) return;
            if( NodeId1.Value >= Simulator.Chains[ChainId1.Value].Nodes.Count ) return;
            if( NodeId2.Value >= Simulator.Chains[ChainId2.Value].Nodes.Count ) return;

            var node1 = Simulator.Chains[ChainId1.Value].Nodes[NodeId1.Value];
            var node2 = Simulator.Chains[ChainId2.Value].Nodes[NodeId2.Value];

            if( !boneMatrixes.TryGetValue( node1.BoneName.Value, out var bone1 ) ) return;
            if( !boneMatrixes.TryGetValue( node2.BoneName.Value, out var bone2 ) ) return;

            var pos1 = bone1.BindPose.TranslationVector;
            var pos2 = bone2.BindPose.TranslationVector;

            simulation.AddCylinder( pos1, pos2, CollisionRadius.Value * 2f, 10 );
        }
    }
}
