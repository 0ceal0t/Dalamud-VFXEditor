using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.PhybFormat.Simulator.CollisionData {
    public enum CollisionType : int {
        Both = 0,
        Outside = 1,
        Inside = 2
    }

    public class PhybCollisionData : PhybData, IPhysicsObject {
        public readonly PhybSimulator Simulator;

        public readonly ParsedPaddedString Name = new( "Name", "replace_me", 32, 0xFE );
        public readonly ParsedEnum<CollisionType> Type = new( "Collision Type" );

        public PhybCollisionData( PhybFile file, PhybSimulator simulator ) : base( file ) {
            Simulator = simulator;
        }

        public PhybCollisionData( PhybFile file, PhybSimulator simulator, BinaryReader reader ) : base( file, reader ) {
            Simulator = simulator;
        }

        protected override List<ParsedBase> GetParsed() => new() {
            Name,
            Type,
        };

        public void AddPhysicsObjects( MeshBuilder collision, MeshBuilder simulation, Dictionary<string, Bone> boneMatrixes ) {

        }
    }
}
