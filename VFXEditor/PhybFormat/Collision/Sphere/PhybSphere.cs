using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.PhybFormat.Collision.Sphere {
    public class PhybSphere : PhybData {
        public readonly ParsedPaddedString Name = new( "Name", 32, 0xFE );
        public readonly ParsedPaddedString Bone = new( "Bone", 32, 0xFE );
        public readonly ParsedFloat3 BoneOffset = new( "Bone Offset" );
        public readonly ParsedFloat Thickness = new( "Thickness" );

        public PhybSphere( PhybFile file ) : base( file ) { }

        public PhybSphere( PhybFile file, BinaryReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => new() {
            Name,
            Bone,
            BoneOffset,
            Thickness,
        };
    }
}
