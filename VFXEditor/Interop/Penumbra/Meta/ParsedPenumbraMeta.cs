using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace VfxEditor.Interop.Penumbra.Meta {
    public class ParsedPenumbraMeta {
        public ParsedPenumbraMeta( string base64 ) {
            var bytes = Convert.FromBase64String( base64 );
            using var compressedStream = new MemoryStream( bytes );
            using var zipStream = new GZipStream( compressedStream, CompressionMode.Decompress );
            using var resultStream = new MemoryStream();
            zipStream.CopyTo( resultStream );
            resultStream.Flush();
            resultStream.Position = 0;

            using var reader = new BinaryReader( resultStream );
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

        private void ParseV1( BinaryReader reader ) {
            reader.ReadBytes( 8 ); // MAGIC = META0001

        }

        private void ParseV2( BinaryReader reader ) {

        }
    }
}