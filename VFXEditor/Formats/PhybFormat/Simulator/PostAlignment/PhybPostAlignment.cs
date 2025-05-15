using HelixToolkit.SharpDX.Core.Animations;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.PhybFormat.Simulator.PostAlignment {
    public class PhybPostAlignment : PhybData, IPhysicsObject {
        public readonly PhybSimulator Simulator;

        public readonly ParsedPaddedString CollisionName = new( "Collision Name", 32, 0xFE );
        public readonly ParsedShort ChainId = new( "Chain Id" );
        public readonly ParsedShort NodeId = new( "Node Id" );

        public PhybPostAlignment( PhybFile file, PhybSimulator simulator ) : base( file ) {
            Simulator = simulator;
        }

        public PhybPostAlignment( PhybFile file, PhybSimulator simulator, BinaryReader reader ) : base( file, reader ) {
            Simulator = simulator;
        }

        protected override List<ParsedBase> GetParsed() => [
            CollisionName,
            ChainId,
            NodeId,
        ];

        public void AddPhysicsObjects( MeshBuilders meshes, Dictionary<string, Bone> boneMatrixes ) {

        }

        public PhybPostAlignment Clone(PhybFile newFile, PhybSimulator newSimulator) {
            var clone = new PhybPostAlignment(newFile, newSimulator);
            clone.CollisionName.Value = CollisionName.Value;
            clone.ChainId.Value = ChainId.Value;
            clone.NodeId.Value = NodeId.Value;
            return clone;
        }
    }
}
