using System;
using System.IO;
using System.IO.Compression;
using VfxEditor.Ui.Export;

namespace VfxEditor.Utils {
    public static class TexToolsUtils {
        public static TTMPL_Simple CreateModResource( string path, int modOffset, int modSize ) {
            var simple = new TTMPL_Simple();
            var split = path.Split( '/' );
            simple.Name = split[^1];
            simple.Category = "Raw File Import";
            simple.FullPath = path;
            simple.IsDefault = false;
            simple.ModOffset = modOffset;
            simple.ModSize = modSize;

            switch( split[0] ) {
                case "vfx":
                    simple.DatFile = "080000";
                    break;
                case "bgcommon":
                    simple.DatFile = "010000";
                    break;
                case "cut":
                    simple.DatFile = GetDat( "03", split );
                    break;
                case "chara":
                    simple.DatFile = "040000";
                    break;
                case "shader":
                    simple.DatFile = "050000";
                    break;
                case "ui":
                    simple.DatFile = "060000";
                    break;
                case "sound":
                    simple.DatFile = "070000";
                    break;
                case "music":
                    simple.DatFile = GetDat( "0c", split );
                    break;
                case "bg":
                    simple.DatFile = "02";
                    if( split[1] == "ffxiv" ) simple.DatFile += "0000";
                    else { // bg/ex1/03_abr_a2/dun/a2d1/texture/a2d1_b0_silv02_n.tex
                        simple.DatFile += split[1].Replace( "ex", "" ).PadLeft( 2, '0' ); // expansion
                        simple.DatFile += split[2].Split( '_' )[0].PadLeft( 2, '0' ); // zone
                    }
                    break;
                default:
                    Dalamud.Error( "Invalid path! Could not find DatFile" );
                    break;
            }
            simple.ModPackEntry = null;
            return simple;
        }

        private static string GetDat( string prefix, string[] split ) {
            var ret = prefix;
            if( split[1] == "ffxiv" ) ret += "0000";
            else {
                ret += split[1].Replace( "ex", "" ).PadLeft( 2, '0' ); // expansion
                ret += "00";
            }
            return ret;
        }

        // https://github.com/TexTools/xivModdingFramework/blob/288478772146df085f0d661b09ce89acec6cf72a/xivModdingFramework/SqPack/FileTypes/Dat.cs#L584

        public static byte[] CreateType2Data( byte[] fileData ) {
            using var headerMs = new MemoryStream();
            using var dataMs = new MemoryStream();
            using var header = new BinaryWriter( headerMs );
            using var data = new BinaryWriter( dataMs );

            // Header size is defaulted to 128, but may need to change if the data being imported is very large
            header.Write( 128 );
            header.Write( 2 );
            header.Write( fileData.Length );

            header.Write( 0 ); // placeholders
            header.Write( 0 );

            var dataOffset = 0;
            var totalCompSize = 0;
            var uncompressedLength = fileData.Length;
            var partCount = ( int )Math.Ceiling( uncompressedLength / 16000f );
            header.Write( partCount );

            var remainder = uncompressedLength;
            using( var reader = new BinaryReader( new MemoryStream( fileData ) ) ) {
                reader.BaseStream.Seek( 0, SeekOrigin.Begin );
                for( var i = 1; i <= partCount; i++ ) {
                    if( i == partCount ) {
                        var compressedData = Compressor( reader.ReadBytes( remainder ) );
                        var padding = 128 - ( ( compressedData.Length + 16 ) % 128 );
                        data.Write( 16 );
                        data.Write( 0 );
                        data.Write( compressedData.Length );
                        data.Write( remainder );
                        data.Write( compressedData );
                        data.Write( new byte[padding] );

                        header.Write( dataOffset );
                        header.Write( ( short )( ( compressedData.Length + 16 ) + padding ) );
                        header.Write( ( short )remainder );
                        totalCompSize = dataOffset + ( ( compressedData.Length + 16 ) + padding );
                    }
                    else {
                        var compressedData = Compressor( reader.ReadBytes( 16000 ) );
                        var padding = 128 - ( ( compressedData.Length + 16 ) % 128 );
                        data.Write( 16 );
                        data.Write( 0 );
                        data.Write( compressedData.Length );
                        data.Write( 16000 );
                        data.Write( compressedData );
                        data.Write( new byte[padding] );

                        header.Write( dataOffset );
                        header.Write( ( short )( ( compressedData.Length + 16 ) + padding ) );
                        header.Write( ( short )16000 );
                        dataOffset += ( compressedData.Length + 16 ) + padding;
                        remainder -= 16000;
                    }
                }
            }

            // Save header position
            var savePos = header.BaseStream.Position;

            header.BaseStream.Seek( 12, SeekOrigin.Begin );
            header.Write( totalCompSize / 128 );
            header.Write( totalCompSize / 128 );

            var headerSize = ( int )header.BaseStream.Length;
            var rem = headerSize % 128;
            if( rem != 0 ) headerSize += 128 - rem;

            // Update header size
            header.BaseStream.Seek( 0, SeekOrigin.Begin );
            header.Write( headerSize );

            // Reset header position
            header.BaseStream.Seek( savePos, SeekOrigin.Begin );

            var headerPadding = rem == 0 ? 0 : 128 - rem;
            header.Write( new byte[headerPadding] );

            // Output
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter( ms );
            writer.Write( headerMs.ToArray() );
            writer.Write( dataMs.ToArray() );
            return ms.ToArray();
        }

        // https://github.com/TexTools/xivModdingFramework/blob/288478772146df085f0d661b09ce89acec6cf72a/xivModdingFramework/Helpers/IOUtil.cs#L40

        public static byte[] Compressor( byte[] data ) {
            using var uncompressed = new MemoryStream( data );
            using var ms = new MemoryStream();
            using var deflate = new DeflateStream( ms, CompressionMode.Compress );
            uncompressed.CopyTo( deflate );
            deflate.Close();
            return ms.ToArray();
        }
    }
}
