using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.PhybFormat.Simulator.PostAlignment {
    public class PhybPostAlignment : PhybPhysicsData, IPhysicsObject {
        public readonly ParsedPaddedString CollisionName = new( "Collision Name", 32, 0xFE );
        public readonly ParsedShort ChainId = new( "Chain Id" );
        public readonly ParsedShort JointId = new( "Joint Id" );

        public PhybPostAlignment( PhybFile file ) : base( file ) { }

        public PhybPostAlignment( PhybFile file, BinaryReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => new() {
            CollisionName,
            ChainId,
            JointId,
        };

        public void AddPhysicsObjects( MeshBuilder builder, Dictionary<string, Bone> boneMatrixes ) {

        }
    }
}
