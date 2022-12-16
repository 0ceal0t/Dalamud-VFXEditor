using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VfxEditor.Utils {
    public static class FileUtils {
        public static string ReadString( BinaryReader reader ) {
            var strBytes = new List<byte>();
            int b;
            while( ( b = reader.ReadByte() ) != 0x00 )
                strBytes.Add( ( byte )b );
            return Encoding.ASCII.GetString( strBytes.ToArray() );
        }

        public static string ReadString( BinaryReader reader, int size ) => Encoding.ASCII.GetString( reader.ReadBytes( size ) );

        public static void WriteString( BinaryWriter writer, string str, bool writeNull = false ) {
            writer.Write( Encoding.ASCII.GetBytes( str.Trim().Trim( '\0' ) ) );
            if( writeNull ) writer.Write( ( byte )0 );
        }

        public static bool ShortInput( string id, ref short value ) {
            var val = ( int )value;
            if( ImGui.InputInt( id, ref val ) ) {
                value = ( short )val;
                return true;
            }
            return false;
        }

        public static bool ByteInput( string id, ref byte value ) {
            var val = ( int )value;
            if( ImGui.InputInt( id, ref val ) ) {
                if( val < 0 ) val = 0;
                if( val > 255 ) val = 255;
                value = ( byte )val;
                return true;
            }
            return false;
        }

        public static byte[] GetOriginal( BinaryReader reader ) {
            var savePos = reader.BaseStream.Position;
            var res = ReadAllBytes( reader );
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
            return res;
        }

        public static byte[] ReadAllBytes( BinaryReader reader ) {
            const int bufferSize = 4096;
            using var ms = new MemoryStream();
            var buffer = new byte[bufferSize];
            int count;
            while( ( count = reader.Read( buffer, 0, buffer.Length ) ) != 0 )
                ms.Write( buffer, 0, count );
            return ms.ToArray();
        }

        public static bool CompareFiles( byte[] original, byte[] data, out int diffIdx ) => CompareFiles( original, data, -1, out diffIdx );

        public static bool CompareFiles( byte[] original, byte[] data, int minIdx, out int diffIdx ) {
            diffIdx = -1;
            for( var idx = 0; idx < Math.Min( data.Length, original.Length ); idx++ ) {
                if( idx > minIdx && data[idx] != original[idx] ) {
                    diffIdx = idx;
                    PluginLog.Log( $"Warning: files do not match at {idx} {data[idx]} {original[idx]}" );
                    return false;
                }
            }
            return true;
        }

        public static long PadTo( BinaryWriter writer, long multiple ) => PadTo( writer, writer.BaseStream.Position, multiple );

        public static long PadTo( BinaryWriter writer, long position, long multiple ) {
            var paddedBytes = ( position % multiple == 0 ) ? 0 : ( multiple - ( position % multiple ) );
            for( var j = 0; j < paddedBytes; j++ ) writer.Write( ( byte )0 );
            return paddedBytes;
        }

        public static long PadTo( BinaryReader reader, long multiple ) => PadTo( reader, reader.BaseStream.Position, multiple );

        public static long PadTo( BinaryReader reader, long position, long multiple ) {
            var paddedBytes = ( position % multiple == 0 ) ? 0 : ( multiple - ( position % multiple ) );
            for( var j = 0; j < paddedBytes; j++ ) reader.ReadByte();
            return paddedBytes;
        }
    }
}
