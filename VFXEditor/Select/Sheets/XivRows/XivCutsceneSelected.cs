using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VFXSelect.Data.Rows {
    public class XivCutsceneSelected {
        public XivCutscene Cutscene;

        public HashSet<string> VfxPaths = new();

        public static readonly Regex rx = new( @"\u0000([a-zA-Z0-9\/_]*?)\.avfx", RegexOptions.Compiled );

        public XivCutsceneSelected( XivCutscene cutscene, Lumina.Data.FileResource file ) {
            Cutscene = cutscene;

            var data = file.Data;
            var stringData = Encoding.UTF8.GetString( data );
            var matches = rx.Matches( stringData );
            foreach( Match m in matches ) {
                VfxPaths.Add( m.Value.Trim( '\u0000' ) );
            }
        }
    }
}
