using System;
using System.Diagnostics;
using System.IO;

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
    }
}