using Dalamud.Logging;
using System;
using System.Collections.Generic;
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
        public static byte[] CreateType2Data( byte[] dataToCreate ) {
            var newData = new List<byte>();
            var headerData = new List<byte>();
            var dataBlocks = new List<byte>();
            // Header size is defaulted to 128, but may need to change if the data being imported is very large.
            headerData.AddRange( BitConverter.GetBytes( 128 ) );
            headerData.AddRange( BitConverter.GetBytes( 2 ) );
            headerData.AddRange( BitConverter.GetBytes( dataToCreate.Length ) );
            var dataOffset = 0;
            var totalCompSize = 0;
            var uncompressedLength = dataToCreate.Length;
            var partCount = ( int )Math.Ceiling( uncompressedLength / 16000f );
            headerData.AddRange( BitConverter.GetBytes( partCount ) );
            var remainder = uncompressedLength;
            using( var binaryReader = new BinaryReader( new MemoryStream( dataToCreate ) ) ) {
                binaryReader.BaseStream.Seek( 0, SeekOrigin.Begin );
                for( var i = 1; i <= partCount; i++ ) {
                    if( i == partCount ) {
                        var compressedData = Compressor( binaryReader.ReadBytes( remainder ) );
                        var padding = 128 - ( ( compressedData.Length + 16 ) % 128 );
                        dataBlocks.AddRange( BitConverter.GetBytes( 16 ) );
                        dataBlocks.AddRange( BitConverter.GetBytes( 0 ) );
                        dataBlocks.AddRange( BitConverter.GetBytes( compressedData.Length ) );
                        dataBlocks.AddRange( BitConverter.GetBytes( remainder ) );
                        dataBlocks.AddRange( compressedData );
                        dataBlocks.AddRange( new byte[padding] );
                        headerData.AddRange( BitConverter.GetBytes( dataOffset ) );
                        headerData.AddRange( BitConverter.GetBytes( ( short )( ( compressedData.Length + 16 ) + padding ) ) );
                        headerData.AddRange( BitConverter.GetBytes( ( short )remainder ) );
                        totalCompSize = dataOffset + ( ( compressedData.Length + 16 ) + padding );
                    }
                    else {
                        var compressedData = Compressor( binaryReader.ReadBytes( 16000 ) );
                        var padding = 128 - ( ( compressedData.Length + 16 ) % 128 );
                        dataBlocks.AddRange( BitConverter.GetBytes( 16 ) );
                        dataBlocks.AddRange( BitConverter.GetBytes( 0 ) );
                        dataBlocks.AddRange( BitConverter.GetBytes( compressedData.Length ) );
                        dataBlocks.AddRange( BitConverter.GetBytes( 16000 ) );
                        dataBlocks.AddRange( compressedData );
                        dataBlocks.AddRange( new byte[padding] );
                        headerData.AddRange( BitConverter.GetBytes( dataOffset ) );
                        headerData.AddRange( BitConverter.GetBytes( ( short )( ( compressedData.Length + 16 ) + padding ) ) );
                        headerData.AddRange( BitConverter.GetBytes( ( short )16000 ) );
                        dataOffset += ( ( compressedData.Length + 16 ) + padding );
                        remainder -= 16000;
                    }
                }
            }
            headerData.InsertRange( 12, BitConverter.GetBytes( totalCompSize / 128 ) );
            headerData.InsertRange( 16, BitConverter.GetBytes( totalCompSize / 128 ) );
            var headerSize = headerData.Count;
            var rem = headerSize % 128;
            if( rem != 0 ) {
                headerSize += ( 128 - rem );
            }
            headerData.RemoveRange( 0, 4 );
            headerData.InsertRange( 0, BitConverter.GetBytes( headerSize ) );
            var headerPadding = rem == 0 ? 0 : 128 - rem;
            headerData.AddRange( new byte[headerPadding] );
            newData.AddRange( headerData );
            newData.AddRange( dataBlocks );
            return newData.ToArray();
        }

        // https://github.com/TexTools/xivModdingFramework/blob/288478772146df085f0d661b09ce89acec6cf72a/xivModdingFramework/Helpers/IOUtil.cs#L40
        public static byte[] Compressor( byte[] uncompressedBytes ) {
            using var uMemoryStream = new MemoryStream( uncompressedBytes );
            byte[] compbytes = null;
            using( var cMemoryStream = new MemoryStream() )
            using( var deflateStream = new DeflateStream( cMemoryStream, CompressionMode.Compress ) ) {
                uMemoryStream.CopyTo( deflateStream );
                deflateStream.Close();
                compbytes = cMemoryStream.ToArray();
            }
            return compbytes;
        }
    }
}
