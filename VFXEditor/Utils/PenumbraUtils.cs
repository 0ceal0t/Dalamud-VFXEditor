using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Utils {
    public static class PenumbraUtils {
        public static void WriteBytes( byte[] data, string modRootFolder, string groupOption, string gamePath, Dictionary<string, string> files ) {
            var filePath = string.IsNullOrEmpty( groupOption ) ? gamePath : Path.Combine( groupOption.ToLower(), gamePath );
            var path = Path.Combine( modRootFolder, filePath );
            Directory.CreateDirectory( Path.GetDirectoryName( path ) );
            File.WriteAllBytes( path, data );

            files[gamePath] = filePath.Replace( '/', '\\' );
        }

        public static void CopyFile( string localPath, string modRootFolder, string groupOption, string gamePath, Dictionary<string, string> files ) {
            var filePath = string.IsNullOrEmpty( groupOption ) ? gamePath : Path.Combine( groupOption.ToLower(), gamePath );
            var path = Path.Combine( modRootFolder, filePath );
            Directory.CreateDirectory( Path.GetDirectoryName( path ) );
            File.Copy( localPath, path, true );

            files[gamePath] = filePath.Replace( '/', '\\' );
        }
    }
}
