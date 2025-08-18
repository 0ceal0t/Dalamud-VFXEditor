using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using VfxEditor.Select;

namespace VfxEditor.FileManager.Interfaces {
    public enum ResetType {
        Reset,
        PluginClosing,
        ToDefault
    }

    public interface IFileManager : IFileManagerSelect {
        public bool DoDebug( string path );

        public bool GetReplacePath( string gamePath, out string replacePath );

        public bool FileExists( string path );

        public bool AcceptsExt( string path );

        public void PenumbraImport( SelectResult selectedFile, SelectResult replacedFile );

        public void WorkspaceImport( JObject meta, string loadLocation );

        public void WorkspaceExport( Dictionary<string, string> meta, string saveLocation );

        public IEnumerable<IFileDocument> GetDocuments();

        public string GetName();

        public void Show();

        public void Draw();

        public void Reset( ResetType type );

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
