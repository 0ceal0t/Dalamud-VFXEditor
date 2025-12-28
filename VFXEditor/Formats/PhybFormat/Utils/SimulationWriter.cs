using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Parsing.Utils;
using VfxEditor.PhybFormat.Simulator;
using VfxEditor.PhybFormat.Simulator.PostAlignment;
using static CommunityToolkit.Mvvm.ComponentModel.__Internals.__TaskExtensions.TaskAwaitableWithoutEndValidation;

namespace VfxEditor.PhybFormat.Utils {
    public class SimulationWriter : ParsingWriter {
        private struct OffsetStruct {
            public long PlaceholderPos; // in Writer
            public long ExtraPos; // in Extra
        }

        public readonly BinaryWriter ExtraWriter;
        public readonly MemoryStream ExtraMs;

        private readonly List<OffsetStruct> Offsets = [];

        public SimulationWriter() : base() {
            ExtraMs = new();
            ExtraWriter = new( ExtraMs );
        }

        public void Write(List<PhybSimulator> simulators ) {
            Write( simulators.Count );

            var placeholders = simulators.Select( x => x.WriteHeader( this ) ).ToList();
            var offsets = new List<List<int>> {
                simulators.Select( x => WriteList( x.Collisions ) ).ToList(),
                simulators.Select( x => WriteList( x.CollisionConnectors ) ).ToList(),
                simulators.Select( x => WriteList( x.Chains ) ).ToList(),
                simulators.Select( x => WriteList( x.Connectors ) ).ToList(),
                simulators.Select( x => WriteList( x.Attracts ) ).ToList(),
                simulators.Select( x => WriteList( x.Pins ) ).ToList(),
                simulators.Select( x => WriteList( x.Springs ) ).ToList(),
                simulators.Select( x => WriteList( x.PostAlignments ) ).ToList(),
            };

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

        public void WritePlaceholder( long offset ) {
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
