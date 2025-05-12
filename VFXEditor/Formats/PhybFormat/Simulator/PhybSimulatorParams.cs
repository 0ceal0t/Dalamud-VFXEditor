using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.PhybFormat.Simulator {
    [Flags]
    public enum SimulatorFlags {
        Simulating = 0x01,
        Collisions_Handled = 0x02,
        Continuous_Collisions = 0x04,
        Using_Ground_Plane = 0x08,
        Fixed_Length = 0x10,
    }

    public class PhybSimulatorParams : PhybData {
        public readonly ParsedFloat3 Gravity = new( "Gravity" );
        public readonly ParsedFloat3 Wind = new( "Wind" );
        public readonly ParsedShort ConstraintLoop = new( "Constraint Loop" );
        public readonly ParsedShort CollisionLoop = new( "Collision Loop" );
        public readonly ParsedFlag<SimulatorFlags> Flags = new( "Flags", size: 1 );
        public readonly ParsedByte Group = new( "Group" );
        public readonly ParsedReserve Padding = new( 2 );

        public PhybSimulatorParams( PhybFile file ) : base( file ) { }

        public PhybSimulatorParams( PhybFile file, BinaryReader reader ) : base( file, reader ) { }

        public void CopyFrom(PhybSimulatorParams other) {
            Gravity.Value = other.Gravity.Value;
            Wind.Value = other.Wind.Value;
            ConstraintLoop.Value = other.ConstraintLoop.Value;
            CollisionLoop.Value = other.CollisionLoop.Value;
            Flags.Value = other.Flags.Value;
            Group.Value = other.Group.Value;
        }

        protected override List<ParsedBase> GetParsed() => new() {
            Gravity,
            Wind,
            ConstraintLoop,
            CollisionLoop,
            Flags,
            Group,
            Padding,
        };
    }
}
