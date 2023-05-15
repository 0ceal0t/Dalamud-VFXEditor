using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Utils {
    public static class PenumbraUtils {
        public static void WriteBytes( byte[] data, string modFolder, string path, Dictionary<string, string> files ) {
            var modFile = Path.Combine( modFolder, path );
            var modFileFolder = Path.GetDirectoryName( modFile );
            Directory.CreateDirectory( modFileFolder );
            File.WriteAllBytes( modFile, data );

            files[path] = path.Replace( '/', '\\' );
        }

        public static void CopyFile( string localPath, string modFolder, string path, Dictionary<string, string> files ) {
            var modFile = Path.Combine( modFolder, path );
            var modFileFolder = Path.GetDirectoryName( modFile );
            Directory.CreateDirectory( modFileFolder );
            File.Copy( localPath, modFile, true );

            files[path] = path.Replace( '/', '\\' );
        }
    }
}
