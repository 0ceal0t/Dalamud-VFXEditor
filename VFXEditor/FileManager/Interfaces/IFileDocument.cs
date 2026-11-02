using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Export;

namespace VfxEditor.FileManager.Interfaces {
    public interface IFileDocument {
        bool GetActiveReplacement( out string gamePath, out string localPath );

        string GetExportSource();

        string GetExportReplace();

        bool CanExport();

        void TextoolsExport( BinaryWriter writer, List<TTMPL_Simple> simplePartsOut, ref int modOffset );

        void PenumbraExport( string modFolder, string groupOption, Dictionary<string, string> filesOut );

        bool GetReplacePath( string path, out string replacePath );
    }
}