using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.PhybFormat.Simulator.Spring {
    public class PhybSpring : PhybPhysicsData, IPhysicsObject {
        public readonly PhybSimulator Simulator;

        public readonly ParsedShort ChainId1 = new( "Chain Id 1" );
        public readonly ParsedShort ChainId2 = new( "Chain Id 2" );
        public readonly ParsedShort NodeId1 = new( "Node Id 1" );
        public readonly ParsedShort NodeId2 = new( "Node Id 2" );
        public readonly ParsedFloat StretchStiffness = new( "Stretch Stiffness" );
        public readonly ParsedFloat CompressStiffness = new( "Compress Stiffness" );

        public PhybSpring( PhybFile file, PhybSimulator simulator ) : base( file ) {
            Simulator = simulator;
        }

        public PhybSpring( PhybFile file, PhybSimulator simulator, BinaryReader reader ) : base( file, reader ) {
            Simulator = simulator;
        }

        protected override List<ParsedBase> GetParsed() => new() {
            ChainId1,
            ChainId2,
            NodeId1,
            NodeId2,
            StretchStiffness,
            CompressStiffness,
        };

        public void AddPhysicsObjects( MeshBuilder collision, MeshBuilder simulation, Dictionary<string, Bone> boneMatrixes ) {

        }
    }
}
