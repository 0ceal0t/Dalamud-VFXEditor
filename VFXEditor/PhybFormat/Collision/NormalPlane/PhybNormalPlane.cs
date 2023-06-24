using HelixToolkit.SharpDX.Core.Animations;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.PhybFormat.Collision.NormalPlane {
    public class PhybNormalPlane : PhybPhysicsData, IPhysicsObject {
        public readonly ParsedPaddedString Name = new( "Name", "replace_me", 32, 0xFE );
        public readonly ParsedPaddedString Bone = new( "Bone", 32, 0xFE );
        public readonly ParsedFloat3 BoneOffset = new( "Bone Offset" );
        public readonly ParsedFloat3 Normal = new( "Normal" );
        public readonly ParsedFloat Thickness = new( "Thickness" );

        public PhybNormalPlane( PhybFile file ) : base( file ) { }

        public PhybNormalPlane( PhybFile file, BinaryReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => new() {
            Name,
            Bone,
            BoneOffset,
            Normal,
            Thickness,
        };

        public void AddPhysicsObjects( MeshBuilders meshes, Dictionary<string, Bone> boneMatrixes ) {

        }
    }
}
