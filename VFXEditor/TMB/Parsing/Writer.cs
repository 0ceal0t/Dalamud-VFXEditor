using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.TMB.Parsing {
    public class Writer {
        public enum ContextType {
            HEADER,
            ENTRIES,
            EXTRA,
            TIMELINES,
            STRING
        }

        private struct WriterPointer {
            public long Position; // position to use for offset calculation
            public long WritePosition; // where in the binarywriter to write
            public ContextType Type;
        }

        public readonly BinaryWriter HeaderWriter;
        public readonly BinaryWriter EntriesWriter;
        public readonly BinaryWriter ExtraWriter;
        public readonly BinaryWriter TimelineWriter;
        public readonly BinaryWriter StringWriter;

        private readonly MemoryStream HeaderMs;
        private readonly MemoryStream EntriesMs;
        private readonly MemoryStream ExtraMs;
        private readonly MemoryStream TimelineMs;
        private readonly MemoryStream StringMs;

        public long EntriesPos => HeaderMs.Length;
        public long ExtraPos => EntriesPos + EntriesMs.Length;
        public long TimelinePos => ExtraPos + ExtraMs.Length;
        public long StringPos => TimelinePos + TimelineMs.Length;
        public int TotalSize => (int) (StringPos + StringMs.Length);

        private Dictionary<WriterPointer, WriterPointer> OffsetPlaceholders = new();

        public Writer() {
            HeaderMs = new MemoryStream();
            EntriesMs = new MemoryStream();
            ExtraMs = new MemoryStream();
            TimelineMs = new MemoryStream();
            StringMs = new MemoryStream();

            HeaderWriter = new BinaryWriter( HeaderMs );
            EntriesWriter = new BinaryWriter( EntriesMs );
            ExtraWriter = new BinaryWriter( ExtraMs );
            TimelineWriter = new BinaryWriter( TimelineMs );
            StringWriter = new BinaryWriter( StringMs );
        }

        public void AddOffset( long startPosition, ContextType startType, ContextType endType ) {
            var startWriter = GetWriter( startType );
            var endWriter = GetWriter( endType );

            var startContext = new WriterPointer {
                Position = startPosition,
                WritePosition = startWriter.BaseStream.Position,
                Type = startType
            };

            var endContext = new WriterPointer {
                Position = endWriter.BaseStream.Position,
                WritePosition = 0,
                Type = endType
            };

            OffsetPlaceholders.Add( startContext, endContext );
        }

        private long GetResolvedPosition( long position, ContextType type ) =>
            position + type switch {
                ContextType.HEADER => 0,
                ContextType.ENTRIES => EntriesPos,
                ContextType.EXTRA => ExtraPos,
                ContextType.TIMELINES => TimelinePos,
                ContextType.STRING => StringPos,
                _ => 0
            };

        private BinaryWriter GetWriter( ContextType type ) =>
            type switch {
                ContextType.HEADER => HeaderWriter,
                ContextType.ENTRIES => EntriesWriter,
                ContextType.EXTRA => ExtraWriter,
                ContextType.TIMELINES => TimelineWriter,
                ContextType.STRING => StringWriter,
                _ => null
            };

        public byte[] ToBytes() {
            using MemoryStream outputMs = new();
            using BinaryWriter outputWriter = new( outputMs );

            outputWriter.Write( HeaderMs.ToArray() );
            outputWriter.Write( EntriesMs.ToArray() );
            outputWriter.Write( ExtraMs.ToArray() );
            outputWriter.Write( TimelineMs.ToArray() );
            outputWriter.Write( StringMs.ToArray() );

            return outputMs.ToArray();
        }

        public void Dispose() {
            HeaderWriter.Close();
            EntriesWriter.Close();
            TimelineWriter.Close();
            StringWriter.Close();

            HeaderMs.Close();
            EntriesMs.Close();
            ExtraMs.Close();
            TimelineMs.Close();
            StringMs.Close();
        }
    }
}
