using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.PhybFormat.Simulator.Attract {
    public class PhybAttract : PhybPhysicsData, IPhysicsObject {
        public readonly ParsedPaddedString BoneName = new( "Bone Name", 32, 0xFE );
        public readonly ParsedFloat3 BoneOffset = new( "Bone Offset" );
        public readonly ParsedShort ChainId = new( "Chain Id" );
        public readonly ParsedShort JointId = new( "Joint Id" );
        public readonly ParsedFloat Stiffness = new( "Stiffness" );

        public PhybAttract( PhybFile file ) : base( file ) { }

        public PhybAttract( PhybFile file, BinaryReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => new() {
            BoneName,
            BoneOffset,
            ChainId,
            JointId,
            Stiffness,
        };

        public void AddPhysicsObjects( MeshBuilder builder, Dictionary<string, Bone> boneMatrixes ) {

        }
    }
}
