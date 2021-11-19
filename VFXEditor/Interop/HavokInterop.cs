using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Interop {
    public class HavokInterop {
        public static void ReplaceHavokAnimation( string baseHkx, int indexToReplace, string newHkx, int indexToUse, string output) {
            // Use ProcessStartInfo class
            var startInfo = new ProcessStartInfo {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = Path.Combine( Plugin.TemplateLocation, "Files", "animassist.exe" ),
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = $"{baseHkx} {indexToReplace} {newHkx} {indexToUse} {output}"
            };

            try {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using var exeProcess = Process.Start( startInfo );
                exeProcess.WaitForExit();
            }
            catch {
                // Log error.
            }
        }
    }
}
