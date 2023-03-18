using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VfxEditor.Select.Rows {
    public partial class XivCutsceneSelected {
        public readonly XivCutscene Cutscene;
        public readonly HashSet<string> VfxPaths = new();

        public XivCutsceneSelected( XivCutscene cutscene, Lumina.Data.FileResource file ) {
            Cutscene = cutscene;

            var data = file.Data;
            var stringData = Encoding.UTF8.GetString( data );
            var matches = SheetManager.AvfxRegex.Matches( stringData );
            foreach( var m in matches.Cast<Match>() ) {
                VfxPaths.Add( m.Value.Trim( '\u0000' ) );
            }
        }
    }
}
