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

        public readonly ParsedPaddedString CollisionName = new( "Collision Name", "replace_me", 32, 0xFE );
        public readonly ParsedEnum<CollisionType> Type = new( "Collision Type" );

        public PhybCollisionData( PhybFile file, PhybSimulator simulator ) : base( file ) {
            Simulator = simulator;
        }

        public PhybCollisionData( PhybFile file, PhybSimulator simulator, BinaryReader reader ) : base( file, reader ) {
            Simulator = simulator;
        }

        protected override List<ParsedBase> GetParsed() => new() {
            CollisionName,
            Type,
        };

        public void AddPhysicsObjects( MeshBuilders meshes, Dictionary<string, Bone> boneMatrixes ) {

        }

        public PhybCollisionData Clone(PhybFile newFile, PhybSimulator newSimulator) {
            var clone = new PhybCollisionData(newFile, newSimulator);
            clone.CollisionName.Value = CollisionName.Value;
            clone.Type.Value = Type.Value;
            return clone;
        }
    }
}
