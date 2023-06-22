using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.PhybFormat.Simulator.PostAlignment {
    public class PhybPostAlignment : PhybData {
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
    }
}
