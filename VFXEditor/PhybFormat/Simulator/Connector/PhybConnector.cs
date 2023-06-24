using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.PhybFormat.Simulator.Connector {
    public class PhybConnector : PhybPhysicsData, IPhysicsObject {
        public readonly ParsedShort ChainId1 = new( "Chain Id 1" );
        public readonly ParsedShort ChainId2 = new( "Chain Id 2" );
        public readonly ParsedShort JointId1 = new( "Joint Id 1" );
        public readonly ParsedShort JointId2 = new( "Joint Id 2" );
        public readonly ParsedFloat CollisionRadius = new( "Collision Radius" );
        public readonly ParsedFloat Friction = new( "Friction" );
        public readonly ParsedFloat Dampening = new( "Dampening" );
        public readonly ParsedFloat Repulsion = new( "Repulsion" );
        public readonly ParsedUInt CollisionFlag = new( "Collision Flags" );
        public readonly ParsedUInt ContinuousCollisionFlag = new( "Continuous Collision Flags" );

        public PhybConnector( PhybFile file ) : base( file ) { }

        public PhybConnector( PhybFile file, BinaryReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => new() {
            ChainId1,
            ChainId2,
            JointId1,
            JointId2,
            CollisionRadius,
            Friction,
            Dampening,
            Repulsion,
            CollisionFlag,
            ContinuousCollisionFlag,
        };

        public void AddPhysicsObjects( MeshBuilder builder, Dictionary<string, Bone> boneMatrixes ) {

        }
    }
}
