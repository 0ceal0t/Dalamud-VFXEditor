using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace VfxEditor.FileManager.Interfaces {
    public interface IFileManager : IFileManagerSelect {
        public string NewWriteLocation { get; }

        public bool DoDebug( string path );
        public bool GetReplacePath( string gamePath, out string replacePath );

        public void WorkspaceImport( JObject meta, string loadLocation );
        public void WorkspaceExport( Dictionary<string, string> meta, string saveLocation );

        public IEnumerable<IFileDocument> GetDocuments();

        public void Show();
        public void Draw();

        public void ToDefault();
        public void Dispose();

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
