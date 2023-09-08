using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing.Utils;

namespace VfxEditor.PhybFormat.Utils {
    public class SimulationWriter : ParsingWriter {
        private struct OffsetStruct {
            public long PlaceholderPos; // in Writer
            public long ExtraPos; // in Extra
        }

        public readonly BinaryWriter ExtraWriter;
        public readonly MemoryStream ExtraMs;

        private readonly List<OffsetStruct> Offsets = new();

        public SimulationWriter() : base() {
            ExtraMs = new();
            ExtraWriter = new( ExtraMs );
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
                Writer.BaseStream.Seek( offset.PlaceholderPos, SeekOrigin.Begin );

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
