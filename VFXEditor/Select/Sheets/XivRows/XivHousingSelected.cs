using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VFXSelect.Data.Rows {
    public class XivHousingSelected {
        public XivHousing Housing;

        public HashSet<string> VfxPaths = new HashSet<string>();
        public static Regex rx = new Regex( @"\u0000([a-zA-Z0-9\/_]*?)\.avfx", RegexOptions.Compiled );

        public XivHousingSelected( XivHousing housing, Lumina.Data.FileResource file ) {
            Housing = housing;

            var data = file.Data;
            string stringData = Encoding.UTF8.GetString( data );
            MatchCollection matches = rx.Matches( stringData );
            foreach( Match m in matches ) {
                VfxPaths.Add( m.Value.Trim('\u0000') );
            }
        }
    }
}
