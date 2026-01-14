using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Parsing.Utils;
using VfxEditor.PhybFormat.Simulator;
using VfxEditor.PhybFormat.Simulator.PostAlignment;

namespace VfxEditor.PhybFormat.Utils {
    public class SimulationWriter : ParsingWriter {
        private struct OffsetStruct {
            public long PlaceholderPos; // in Writer
            public long ExtraPos; // in Extra
        }

        public readonly BinaryWriter ExtraWriter;
        public readonly MemoryStream ExtraMs;

        public readonly long SimulatorOffset;
        public readonly Queue<long> ChainCollisionData = new();

        private readonly List<OffsetStruct> Offsets = [];

        public SimulationWriter( long simulatorOffset ) : base() {
            ExtraMs = new();
            ExtraWriter = new( ExtraMs );
            SimulatorOffset = simulatorOffset;
        }

        public void Write(List<PhybSimulator> simulators ) {
            Write( simulators.Count );

            var placeholders = simulators.Select( x => x.WriteHeader( this ) ).ToList();

            var offsets = new List<List<int>> {
                simulators.Select( x => WriteList( x.CollisionObjects ) ).ToList()
            };

            // write the remaining collision connections
            simulators.ForEach( x => x.Chains.ForEach( y => y.WriteCollisionData( this ) ) );

            offsets.AddRange( [
                [.. simulators.Select( x => WriteList( x.CollisionConnections ) )],
                [.. simulators.Select( x => WriteList( x.Chains ) )],
                [.. simulators.Select( x => WriteList( x.Connectors ) )],
                [.. simulators.Select( x => WriteList( x.Attracts ) )],
                [.. simulators.Select( x => WriteList( x.Pins ) )],
                [.. simulators.Select( x => WriteList( x.Springs ) )],
                [.. simulators.Select( x => WriteList( x.PostAlignments ) )],
            ] );

            var resetPos = Position;

            for( var i = 0; i < placeholders.Count; i++ ) {
                Seek( placeholders[i] );
                for( var j = 0; j < offsets.Count; j++ ) {
                    Write( offsets[j][i] );
                }
            }

            Seek( resetPos );
        }

        private int WriteList<T>( List<T> items ) where T : PhybData {
            var offset = Position;
            foreach( var item in items ) item.Write( this );
            var defaultOffset = typeof( T ) == typeof( PhybPostAlignment ) ? 0xCCCCCCCC : 0;
            return items.Count == 0 ? ( int )defaultOffset : ( int )offset - 4;
        }

        public void WriteExtraPlaceholder( long offset ) {
            Offsets.Add( new OffsetStruct {
                PlaceholderPos = Writer.BaseStream.Position,
                ExtraPos = offset
            } );

            Write( 0 ); // placeholder
        }

        public override void WriteTo( BinaryWriter finalWriter ) {
            foreach( var offset in Offsets ) {
                Writer.BaseStream.Position = offset.PlaceholderPos;

                var diff = Writer.BaseStream.Length + offset.ExtraPos;
                Writer.Write( ( int )diff );
            }

            base.WriteTo( finalWriter );
            finalWriter.Write( ExtraMs.ToArray() );
        }

        public override void Dispose() {
            ExtraWriter.Close();
            ExtraMs.Close();
        }
    }
}
