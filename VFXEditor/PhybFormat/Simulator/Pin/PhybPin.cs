using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.PhybFormat.Simulator.Pin {
    public class PhybPin : PhybPhysicsData, IPhysicsObject {
        public readonly ParsedPaddedString BoneName = new( "Bone Name", 32, 0xFE );
        public readonly ParsedFloat3 BoneOffset = new( "Bone Offset" );
        public readonly ParsedShort ChainId = new( "Chain Id" );
        public readonly ParsedShort JointId = new( "Joint Id" );

        public PhybPin( PhybFile file ) : base( file ) { }

        public PhybPin( PhybFile file, BinaryReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => new() {
            BoneName,
            BoneOffset,
            ChainId,
            JointId,
        };

        public void AddPhysicsObjects( MeshBuilder builder, Dictionary<string, Bone> boneMatrixes ) {

        }
    }
}
