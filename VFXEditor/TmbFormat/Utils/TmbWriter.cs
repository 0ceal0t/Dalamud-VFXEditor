using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat.Utils {
    public class TmbWriter {
        public const int BaseSize = 0x10;
        public int BodySize;
        public int ExtraSize;
        public int TimelineSize;
        public int Size => BaseSize + TimelineSize + ExtraSize + ( int )StringWriter.BaseStream.Length;

        public readonly BinaryWriter Writer;
        public readonly BinaryWriter ExtraWriter;
        public readonly BinaryWriter TimelineWriter;
        public readonly BinaryWriter StringWriter;

        public readonly MemoryStream WriterMs;
        public readonly MemoryStream ExtraMs;
        public readonly MemoryStream TimelineMs;
        public readonly MemoryStream StringMs;

        public long StartPosition;
        private readonly Dictionary<string, int> WrittenStrings = new();

        public TmbWriter( int bodySize, int extraSize, int timelineSize ) {
            BodySize = bodySize;
            ExtraSize = extraSize;
            TimelineSize = timelineSize;

            WriterMs = new();
            ExtraMs = new();
            TimelineMs = new();
            StringMs = new();

            Writer = new( WriterMs );
            ExtraWriter = new( ExtraMs );
            TimelineWriter = new( TimelineMs );
            StringWriter = new( StringMs );
        }

        public void Dispose() {
            Writer.Close();
            ExtraWriter.Close();
            TimelineWriter.Close();
            StringWriter.Close();

            WriterMs.Close();
            ExtraMs.Close();
            TimelineMs.Close();
            StringMs.Close();
        }

        public void WriteOffsetString( string str ) {
            if( WrittenStrings.ContainsKey( str ) ) {
                var stringOffset = WrittenStrings[str];
                var actualPos = ( int )( ( BodySize - ( StartPosition + 8 ) ) + ExtraSize + TimelineSize + stringOffset );
                Writer.Write( actualPos );
            }
            else {
                var stringOffset = ( int )StringWriter.BaseStream.Position;
                var actualPos = ( int )( ( BodySize - ( StartPosition + 8 ) ) + ExtraSize + TimelineSize + stringOffset );
                Writer.Write( actualPos );

                FileUtils.WriteString( StringWriter, str, true );
                WrittenStrings[str] = stringOffset;
            }
        }

        public void WriteOffsetTimeline<T>( List<T> entries ) where T : TmbItemWithId {
            var actualPos = ( int )( ( BodySize - ( StartPosition + 8 ) ) + ExtraSize + TimelineWriter.BaseStream.Position );
            Writer.Write( actualPos );
            Writer.Write( entries.Count );

            foreach( var entry in entries ) TimelineWriter.Write( entry.Id );
        }

        public int WriteExtra( Action<BinaryWriter> func, bool writeData ) {
            if( !writeData ) {
                Writer.Write( 0 );
                return -1;
            }
            return WriteExtra( func );
        }

        public int WriteExtra( Action<BinaryWriter> func, int modifyOffset = 0 ) {
            var actualPos = ( int )( ( BodySize - ( StartPosition + 8 + modifyOffset ) ) + ExtraWriter.BaseStream.Position );
            Writer.Write( actualPos );

            func( ExtraWriter );
            return actualPos;
        }

        public void WriteExtraVector4( Vector4 input ) {
            var actualPos = ( int )( ( BodySize - ( StartPosition + 8 ) ) + ExtraWriter.BaseStream.Position );
            Writer.Write( actualPos );
            Writer.Write( 4 );
            ExtraWriter.Write( input.X );
            ExtraWriter.Write( input.Y );
            ExtraWriter.Write( input.Z );
            ExtraWriter.Write( input.W );
        }

        public void WriteExtraVector3( Vector3 input ) {
            var actualPos = ( int )( ( BodySize - ( StartPosition + 8 ) ) + ExtraWriter.BaseStream.Position );
            Writer.Write( actualPos );
            Writer.Write( 3 );
            ExtraWriter.Write( input.X );
            ExtraWriter.Write( input.Y );
            ExtraWriter.Write( input.Z );
        }

        public void Write( int data ) => Writer.Write( data );
        public void Write( short data ) => Writer.Write( data );
        public void Write( byte data ) => Writer.Write( data );
        public void Write( float data ) => Writer.Write( data );
    }
}
