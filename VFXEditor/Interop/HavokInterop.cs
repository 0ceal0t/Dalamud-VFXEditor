using System.Diagnostics;
using System.IO;

namespace VFXEditor.Interop {
    public class HavokInterop {
        public static void ReplaceHavokAnimation( string baseHkx, int indexToReplace, string newHkx, int indexToUse, string output ) {
            Run( $"1 {baseHkx} {indexToReplace} {newHkx} {indexToUse} {output}" );
        }

        public static void RemoveHavokAnimation( string baseHkx, int indexToRemove, string output ) {
            Run( $"2 {baseHkx} {indexToRemove} {baseHkx} {indexToRemove} {output}" );
        }

        public static void AddHavokAnimation( string baseHkx, string newHkx, int indexToUse, string output ) {
            Run( $"3 {baseHkx} {indexToUse} {newHkx} {indexToUse} {output}" );
        }

        public static void Run( string arguments ) {
            // Use ProcessStartInfo class
            var startInfo = new ProcessStartInfo {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = Path.Combine( Plugin.TemplateLocation, "Files", "animassist.exe" ),
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = arguments
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
