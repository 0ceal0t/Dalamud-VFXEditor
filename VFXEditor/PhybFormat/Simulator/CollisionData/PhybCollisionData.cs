using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.PhybFormat.Simulator.CollisionData {
    public enum CollisionType : int {
        Both = 0,
        Outside = 1,
        Inside = 2
    }

    public class PhybCollisionData : PhybData {
        public readonly ParsedPaddedString Name = new( "Name", 32, 0xFE );
        public readonly ParsedEnum<CollisionType> Type = new( "Collision Type" );

        public PhybCollisionData( PhybFile file ) : base( file ) { }

        public PhybCollisionData( PhybFile file, BinaryReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => new() {
            Name,
            Type,
        };
    }
}
