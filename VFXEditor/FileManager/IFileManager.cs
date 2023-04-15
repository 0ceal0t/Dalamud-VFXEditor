using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data;
using VfxEditor.TexTools;

namespace VfxEditor.FileManager {
    public interface IFileManager {
        public bool DoDebug( string path );
        public bool GetReplacePath( string gamePath, out string replacePath );

        public void WorkspaceImport( JObject meta, string loadLocation );
        public void WorkspaceExport( Dictionary<string, string> meta, string saveLocation );
        public void TextoolsExport( BinaryWriter writer, List<TTMPL_Simple> simpleParts, ref int modOffset );
        public void PenumbraExport( string modFolder );

        public string GetExportName();
        public CopyManager GetCopyManager();
        public CommandManager GetCommandManager();

        public void Draw();

        public void ToDefault();
        public void Dispose();

        public void Show();
    }
}
