using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.PhybFormat.Simulator {
    public class PhybSimulatorParams : PhybData {
        public readonly ParsedFloat3 Gravity = new( "Gravity" );
        public readonly ParsedFloat3 Wind = new( "Wind" );
        public readonly ParsedShort ConstraintLoop = new( "Constraint Loop" );
        public readonly ParsedShort CollisionLoop = new( "Collision Loop" );
        public readonly ParsedByteBool Simulating = new( "Simulating" );
        public readonly ParsedByteBool CollisionHandling = new( "Collisions Handled" );
        public readonly ParsedByteBool ContinuousCollision = new( "Continuous Collision" );
        public readonly ParsedByteBool UsingGroundPlane = new( "Using Ground Plane" );
        public readonly ParsedByteBool FixedLength = new( "Fixed Length" );
        public readonly ParsedReserve Padding1 = new( 3 );
        public readonly ParsedByte Group = new( "Group" );
        public readonly ParsedReserve Padding2 = new( 3 );

        public PhybSimulatorParams( PhybFile file ) : base( file ) { }

        public PhybSimulatorParams( PhybFile file, BinaryReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => new() {
            Gravity,
            Wind,
            ConstraintLoop,
            CollisionLoop,
            Simulating,
            CollisionHandling,
            ContinuousCollision,
            UsingGroundPlane,
            FixedLength,
            Padding1,
            Group,
            Padding2,
        };
    }
}
