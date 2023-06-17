using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using VfxEditor.Data;

namespace VfxEditor.FileManager {
    public interface IFileManager {
        public bool DoDebug( string path );
        public bool GetReplacePath( string gamePath, out string replacePath );

        public void WorkspaceImport( JObject meta, string loadLocation );

        public void WorkspaceExport( Dictionary<string, string> meta, string saveLocation );

        public string GetExportName();

        public IEnumerable<IFileDocument> GetExportDocuments();

        public CopyManager GetCopyManager();

        public CommandManager GetCommandManager();

        public void Draw();

        public void ToDefault();
        public void Dispose();

        public void Show();
    }
}
