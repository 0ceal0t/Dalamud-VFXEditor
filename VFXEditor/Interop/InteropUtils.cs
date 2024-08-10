using Dalamud;
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
        public static void Run( string exePath, string arguments, bool captureOutput, out string output ) {
            output = "";

            // Use ProcessStartInfo class
            var startInfo = new ProcessStartInfo {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = Path.Combine( Plugin.RootLocation, "Files", exePath ),
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = arguments,
                RedirectStandardOutput = true
            };

            try {
                var process = new Process {
                    StartInfo = startInfo
                };

                process.Start();
                if( captureOutput ) output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error executing" );
            }
        }

        // https://github.com/xivdev/Penumbra/blob/7710d9249675e6550f9db2eaaf94e1c570929c23/Penumbra/Interop/Hooks/ResourceLoading/ResourceLoader.cs#L269

        public static int ComputeHash( CiByteString path, GetResourceParameters* resParams ) {
            if( resParams == null || !resParams->IsPartialRead )
                return path.Crc32;

            // When the game requests file only partially, crc32 includes that information, in format of:
            // path/to/file.ext.hex_offset.hex_size
            // ex) music/ex4/BGM_EX4_System_Title.scd.381adc.30000
            return CiByteString.Join(
                ( byte )'.',
                path,
                CiByteString.FromString( resParams->SegmentOffset.ToString( "x" ), out var s1, MetaDataComputation.None ) ? s1 : CiByteString.Empty,
                CiByteString.FromString( resParams->SegmentLength.ToString( "x" ), out var s2, MetaDataComputation.None ) ? s2 : CiByteString.Empty
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

        public static byte[] GetDatCategory( uint prefix, string expansion ) {
            var ret = BitConverter.GetBytes( prefix );
            if( expansion == "ffxiv" ) return ret;
            // music/ex4/BGM_EX4_Field_Ult_Day03.scd
            // 04 00 00 0C
            var expansionTrimmed = expansion.Replace( "ex", "" );
            ret[3] = byte.Parse( expansionTrimmed );
            return ret;
        }

        public static void PrepPap( IntPtr resource, List<string> ids, List<short> types ) {
            if( ids == null || types == null ) return;
            Marshal.WriteByte( resource + Constants.PrepPapOffset, Constants.PrepPapValue );
        }

        public static void WritePapIds( IntPtr resource, List<string> ids, List<short> types ) {
            if( ids == null ) return;
            var data = Marshal.ReadIntPtr( resource + Constants.PapIdsOffset );
            for( var i = 0; i < ids.Count; i++ ) {
                SafeMemory.WriteString( data + ( i * 40 ), ids[i], Encoding.ASCII );
                Marshal.WriteInt16( data + ( i * 40 ) + 32, types[i] );
                Marshal.WriteByte( data + ( i * 40 ) + 34, ( byte )i );
            }
        }
    }
}
