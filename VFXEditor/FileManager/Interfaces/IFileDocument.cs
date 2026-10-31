using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Export;

namespace VfxEditor.FileManager.Interfaces {
    public interface IFileDocument {
        public string GetExportSource();

        public string GetExportReplace();

        public bool CanExport();

        public void TextoolsExport( BinaryWriter writer, List<TTMPL_Simple> simplePartsOut, ref int modOffset );

        public void PenumbraExport( string modFolder, string groupOption, Dictionary<string, string> filesOut );

        public bool GetReplacePath( string path, out string replacePath );

        // The game path this document currently replaces, and the local file it is written to. Used to build the Penumbra temporary mod
        public bool GetActiveReplacement( out string gamePath, out string localPath );
    }
}
