using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using VfxEditor.Utils;

namespace VfxEditor.Interop.Penumbra.Meta {
    public class PenumbraMetaParser {
        // TODO: only need IMC for now
        public readonly List<(ImcIdentifier, ImcEntry)> Imc = [];

        public const uint ImcKey = ( ( uint )'I' << 24 ) | ( ( uint )'M' << 16 ) | ( ( uint )'C' << 8 );

        public PenumbraMetaParser( string base64 ) {
            var bytes = Convert.FromBase64String( base64 );
            using var compressedStream = new MemoryStream( bytes );
            using var zipStream = new GZipStream( compressedStream, CompressionMode.Decompress );
            using var resultStream = new MemoryStream();
            zipStream.CopyTo( resultStream );
            resultStream.Flush();
            resultStream.Position = 0;

            var reader = new SpanBinaryReader( resultStream.GetBuffer().AsSpan( 0, ( int )resultStream.Length ) );
            var version = reader.ReadByte();
            switch( version ) {
                case 1:
                    ParseV1( reader );
                    break;
                case 2:
                    ParseV2( reader );
                    break;
                default:
                    Dalamud.Error( $"Invalid version for manipulations: {version}." );
                    break;
            }
        }

        private void ParseV1( SpanBinaryReader reader ) {
            reader.ReadBytes( 8 ); // MAGIC = META0001
            var imcCount = reader.ReadInt32();
            for( var i = 0; i < imcCount; i++ ) {
                var identifier = reader.Read<ImcIdentifier>();
                var value = reader.Read<ImcEntry>();
                Imc.Add( (identifier, value) );
            }
        }

        private void ParseV2( SpanBinaryReader reader ) {
            reader.ReadBytes( 8 ); // MAGIC = META0002
            while( reader.Remaining > 4 ) {
                var prefix = reader.ReadUInt32();
                var count = reader.Remaining > 4 ? reader.ReadInt32() : 0;
                if( count is 0 ) continue;
                switch( prefix ) {
                    case ImcKey:
                        for( var i = 0; i < count; ++i ) {
                            var identifier = reader.Read<ImcIdentifier>();
                            var value = reader.Read<ImcEntry>();
                            Imc.Add( (identifier, value) );
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}