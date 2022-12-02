using Dalamud.Logging;
using System;
using System.Diagnostics;
using System.IO;

namespace VfxEditor.Interop {
    public static class InteropUtils {
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
            catch (Exception e) {
                PluginLog.LogError( e, "Error executing" );
            }
        }
    }
}
