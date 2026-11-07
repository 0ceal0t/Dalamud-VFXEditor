using Dalamud.Interface.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lumina.Data.Files;
using OtterTex;
using VfxEditor.Formats.TextureFormat;

namespace VfxEditor.Utils {
    public static class TextureUtils {
        public static async Task SavePngAsync( byte[] rgba, int width, int height, string path ) {
            try {
                var png = Dalamud.TextureReadbackProvider.GetSupportedImageEncoderInfos().FirstOrDefault( codec =>
                    codec.Extensions.Any( ext => string.Equals( ext, ".png", StringComparison.OrdinalIgnoreCase ) ) ||
                    codec.MimeTypes.Any( mime => string.Equals( mime, "image/png", StringComparison.OrdinalIgnoreCase ) ) )
                    ?? throw new NotSupportedException( "No PNG encoder is available" );

                var wrap = Dalamud.TextureProvider.CreateFromRaw( RawImageSpecification.Rgba32( width, height ), rgba );
                await Dalamud.TextureReadbackProvider.SaveToFileAsync( wrap, png.ContainerGuid, path );
                Dalamud.Log( $"Saved {path}" );
            }
            catch( Exception e ) {
                Dalamud.Error( e, $"Could not save {path}" );
                Dalamud.ErrorNotification( $"Could not save {Path.GetFileName( path )}" );
            }
        }

        // Actual byte size of one mip surface, including block padding for compressed formats
        public static int GetMipSize( TextureFormat format, int width, int height, int mip ) {
            width = Math.Max( width >> mip, 1 );
            height = Math.Max( height >> mip, 1 );
            var dxgi = ( ( TexFile.TextureFormat )format ).ToDXGI();
            if( dxgi == DXGIFormat.Unknown ) throw new NotSupportedException( $"Unsupported texture format {format}" );
            return dxgi.ComputePitch( width, height ).Item2;
        }

        public static void GetDdsInfo( byte[] data, TextureFormat format, int width, int height, int mipCount, out byte[] compressedOut, out List<short> mipPartOffsets, out List<short> mipPartCounts ) {
            using var dataMs = new MemoryStream( data );
            using var reader = new BinaryReader( dataMs );
            using var compressedMs = new MemoryStream();
            using var writer = new BinaryWriter( compressedMs );

            mipPartOffsets = [];
            mipPartCounts = [];

            // already skipped the DDS header

            for( var i = 0; i < mipCount; i++ ) {
                var mipLength = GetMipSize( format, width, height, i );
                var mipParts = ( mipLength + 15999 ) / 16000;
                mipPartCounts.Add( ( short )mipParts );

                for( var j = 0; j < mipParts; j++ ) {
                    GetMipPartOffset( writer, reader, mipPartOffsets, Math.Min( 16000, mipLength - j * 16000 ) );
                }
            }

            compressedOut = compressedMs.ToArray();
        }

        private static void GetMipPartOffset( BinaryWriter writer, BinaryReader reader, List<short> mipPartOffsets, int uncompLength ) {
            var uncompBytes = reader.ReadBytes( uncompLength );
            var compressed = TexToolsUtils.Compressor( uncompBytes );

            var comp = compressed.Length <= uncompLength;
            compressed = comp ? compressed : uncompBytes;

            writer.Write( 16 );
            writer.Write( 0 );
            writer.Write( !comp ? 32000 : compressed.Length );
            writer.Write( uncompLength );
            writer.Write( compressed );

            var padding = 128 - ( compressed.Length % 128 );

            writer.Write( new byte[padding] );

            mipPartOffsets.Add( ( short )( compressed.Length + padding + 16 ) );
        }

        public static byte[] CreateType4Data( TextureFormat format, List<short> mipPartOffsets, List<short> mipPartCount, int uncompressedLength, int mipCount, int width, int height ) {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter( ms );

            var headerSize = 24 + ( mipCount * 20 ) + ( mipPartOffsets.Count * 2 );
            var headerPadding = 128 - ( headerSize % 128 );

            writer.Write( headerSize + headerPadding );
            writer.Write( 4 );
            writer.Write( uncompressedLength + 80 );
            writer.Write( 0 );
            writer.Write( 0 );
            writer.Write( mipCount );

            var partIndex = 0;
            var mipOffsetIndex = 80;

            for( var i = 0; i < mipCount; i++ ) {
                writer.Write( mipOffsetIndex );

                var paddedSize = 0;
                for( var j = 0; j < mipPartCount[i]; j++ ) {
                    paddedSize += mipPartOffsets[j + partIndex];
                }

                writer.Write( paddedSize );
                writer.Write( GetMipSize( format, width, height, i ) );

                writer.Write( partIndex );
                writer.Write( ( int )mipPartCount[i] );

                partIndex += mipPartCount[i];
                mipOffsetIndex += paddedSize;
            }

            foreach( var part in mipPartOffsets ) writer.Write( part );
            writer.Write( new byte[headerPadding] );

            return ms.ToArray();
        }
    }
}