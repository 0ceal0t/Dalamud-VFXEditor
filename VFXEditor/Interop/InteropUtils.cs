using Dalamud;
using Dalamud.Logging;
using Penumbra.String;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using VfxEditor.Structs;

namespace VfxEditor.Interop {
    public static unsafe class InteropUtils {
        public static void Run( string exePath, string arguments ) {
            // Use ProcessStartInfo class
            var startInfo = new ProcessStartInfo {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = Path.Combine( Plugin.RootLocation, "Files", exePath ),
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = arguments
            };

            try {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using var exeProcess = Process.Start( startInfo );
                exeProcess.WaitForExit();
            }
            catch( Exception e ) {
                PluginLog.LogError( e, "Error executing" );
            }
        }

        public static int ComputeHash( ByteString path, GetResourceParameters* resParams ) {
            if( resParams == null || !resParams->IsPartialRead ) return path.Crc32;

            return ByteString.Join(
                ( byte )'.',
                path,
                ByteString.FromStringUnsafe( resParams->SegmentOffset.ToString( "x" ), true ),
                ByteString.FromStringUnsafe( resParams->SegmentLength.ToString( "x" ), true )
            ).Crc32;
        }

        public static byte[] GetBgCategory( string expansion, string zone ) {
            var ret = BitConverter.GetBytes( 2u );
            if( expansion == "ffxiv" ) return ret;
            // ex1/03_abr_a2/fld/a2f1/level/a2f1 -> [02 00 03 01]
            // expansion = ex1
            // zone = 03_abr_a2
            var expansionTrimmed = expansion.Replace( "ex", "" );
            var zoneTrimmed = zone.Split( '_' )[0];
            ret[2] = byte.Parse( zoneTrimmed );
            ret[3] = byte.Parse( expansionTrimmed );
            return ret;
        }

        public static byte[] GetMusicCategory( string expansion ) {
            var ret = BitConverter.GetBytes( 12u );
            if( expansion == "ffxiv" ) return ret;
            // music/ex4/BGM_EX4_Field_Ult_Day03.scd
            // 04 00 00 0C
            var expansionTrimmed = expansion.Replace( "ex", "" );
            ret[3] = byte.Parse( expansionTrimmed );
            return ret;
        }

        public static void PrepPap( IntPtr resource, List<string> papIds ) {
            if( papIds == null ) return;
            Marshal.WriteByte( resource + Constants.PrepPapOffset, Constants.PrepPapValue );
        }

        public static void WritePapIds( IntPtr resource, List<string> papIds ) {
            if( papIds == null ) return;
            var data = Marshal.ReadIntPtr( resource + Constants.PapIdsOffset );
            for( var i = 0; i < papIds.Count; i++ ) {
                SafeMemory.WriteString( data + ( i * 40 ), papIds[i], Encoding.ASCII );
                Marshal.WriteByte( data + ( i * 40 ) + 34, ( byte )i );
            }
        }
    }
}
