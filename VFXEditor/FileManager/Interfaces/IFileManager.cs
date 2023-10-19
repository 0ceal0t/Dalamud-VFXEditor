using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace VfxEditor.FileManager.Interfaces {
    public interface IFileManager : IFileManagerSelect {
        public bool DoDebug( string path );

        public bool GetReplacePath( string gamePath, out string replacePath );

        public bool FileExists( string path );

        public void WorkspaceImport( JObject meta, string loadLocation );

        public void WorkspaceExport( Dictionary<string, string> meta, string saveLocation );

        public IEnumerable<IFileDocument> GetDocuments();

        public void Show();

        public void Draw();

        public void ToDefault();

        public void Reset();

        public static bool FileExist( IFileManager manager, string path ) =>
            Dalamud.GameFileExists( path ) || Plugin.PenumbraIpc.PenumbraFileExists( path, out var _ ) || manager.GetReplacePath( path, out var _ );

        public static bool GetReplacePath( IFileManager manager, string path, out string replacePath ) {
            replacePath = null;
            foreach( var document in manager.GetDocuments() ) {
                if( document.GetReplacePath( path, out var documentReplacePath ) ) {
                    replacePath = documentReplacePath;
                    return true;
                }
            }
            return false;
        }
    }
}
