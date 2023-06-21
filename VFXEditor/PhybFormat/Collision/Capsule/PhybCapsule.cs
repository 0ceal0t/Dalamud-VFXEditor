using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.PhybFormat.Collision.Capsule {
    public class PhybCapsule : PhybData {
        public readonly ParsedPaddedString Name = new( "Name", 32, 0xFE );
        public readonly ParsedPaddedString StartBone = new( "Start Bone", 32, 0xFE );
        public readonly ParsedPaddedString EndBone = new( "End Bone", 32, 0xFE );
        public readonly ParsedFloat3 StartOffset = new( "Start Offset" );
        public readonly ParsedFloat3 EndOffset = new( "End Offset" );
        public readonly ParsedFloat Radius = new( "Radius" );

        public PhybCapsule( PhybFile file ) : base( file ) { }

        public PhybCapsule( PhybFile file, BinaryReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => new() {
            Name,
            StartBone,
            EndBone,
            StartOffset,
            EndOffset,
            Radius,
        };
    }
}
