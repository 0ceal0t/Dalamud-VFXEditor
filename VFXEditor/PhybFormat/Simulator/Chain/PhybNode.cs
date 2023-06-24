using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.PhybFormat.Simulator.Chain {
    public class PhybNode : PhybPhysicsData, IPhysicsObject {
        public readonly PhybSimulator Simulator;

        public readonly ParsedPaddedString BoneName = new( "Bone Name", 32, 0xFE );
        public readonly ParsedFloat Radius = new( "Collision Radius" );
        public readonly ParsedFloat AttractByAnimation = new( "Attract by Animation" );
        public readonly ParsedFloat WindScale = new( "Wind Scale" );
        public readonly ParsedFloat GravityScale = new( "Gravity Scale" );
        public readonly ParsedFloat ConeMaxAngle = new( "Cone Max Angle" );
        public readonly ParsedFloat3 ConeAxisOffset = new( "Cone Axis Offset" );
        public readonly ParsedFloat3 ConstraintPlaneNormal = new( "Constraint Plane Normal" );
        public readonly ParsedUInt CollisionFlag = new( "Collision Flags" );
        public readonly ParsedUInt ContinuousCollisionFlag = new( "Continuous Collision Flags" );

        public PhybNode( PhybFile file, PhybSimulator simulator ) : base( file ) {
            Simulator = simulator;
        }

        public PhybNode( PhybFile file, PhybSimulator simulator, BinaryReader reader ) : base( file, reader ) {
            Simulator = simulator;
        }

        protected override List<ParsedBase> GetParsed() => new() {
            BoneName,
            Radius,
            AttractByAnimation,
            WindScale,
            GravityScale,
            ConeMaxAngle,
            ConeAxisOffset,
            ConstraintPlaneNormal,
            CollisionFlag,
            ContinuousCollisionFlag,
        };

        public void AddPhysicsObjects( MeshBuilder collision, MeshBuilder simulation, Dictionary<string, Bone> boneMatrixes ) {
            if( !boneMatrixes.TryGetValue( BoneName.Value, out var bone ) ) return;
            var pos = bone.BindPose.TranslationVector;
            simulation.AddSphere( pos, Radius.Value, 10, 10 );
        }
    }
}
