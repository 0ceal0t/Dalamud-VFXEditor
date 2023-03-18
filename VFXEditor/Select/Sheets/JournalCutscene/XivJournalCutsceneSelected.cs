using Lumina.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VfxEditor.Select.Rows {
    public partial class XivJournalCutsceneSelected {
        public readonly XivJournalCutscene Cutscene;
        public readonly List<HashSet<string>> VfxPaths = new();

        public XivJournalCutsceneSelected( XivJournalCutscene cutscene, List<FileResource> files ) {
            Cutscene = cutscene;

            foreach( var file in files ) {
                var filePaths = new HashSet<string>();
                var data = file.Data;
                var stringData = Encoding.UTF8.GetString( data );
                var matches = SheetManager.AvfxRegex.Matches( stringData );
                foreach( var m in matches.Cast<Match>() ) {
                    filePaths.Add( m.Value.Trim( '\u0000' ) );
                }
                VfxPaths.Add( filePaths );
            }
        }
    }
}
