using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.PhybFormat.Simulator.Spring {
    public class PhybSpring : PhybPhysicsData, IPhysicsObject {
        public readonly ParsedShort ChainId1 = new( "Chain Id 1" );
        public readonly ParsedShort ChainId2 = new( "Chain Id 2" );
        public readonly ParsedShort JointId1 = new( "Joint Id 1" );
        public readonly ParsedShort JointId2 = new( "Joint Id 2" );
        public readonly ParsedFloat StretchStiffness = new( "Stretch Stiffness" );
        public readonly ParsedFloat CompressStiffness = new( "Compress Stiffness" );

        public PhybSpring( PhybFile file ) : base( file ) { }

        public PhybSpring( PhybFile file, BinaryReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => new() {
            ChainId1,
            ChainId2,
            JointId1,
            JointId2,
            StretchStiffness,
            CompressStiffness,
        };

        public void AddPhysicsObjects( MeshBuilder builder, Dictionary<string, Bone> boneMatrixes ) {

        }
    }
}
