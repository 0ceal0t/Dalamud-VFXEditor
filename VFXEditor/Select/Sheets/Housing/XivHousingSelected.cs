using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VFXEditor.Select.Rows {
    public class XivHousingSelected {
        public XivHousing Housing;

        public HashSet<string> VfxPaths = new();
        public static readonly Regex rx = new( @"\u0000([a-zA-Z0-9\/_]*?)\.avfx", RegexOptions.Compiled );

        public XivHousingSelected( XivHousing housing, Lumina.Data.FileResource file ) {
            Housing = housing;

            var data = file.Data;
            var stringData = Encoding.UTF8.GetString( data );
            var matches = rx.Matches( stringData );
            foreach( Match m in matches ) {
                VfxPaths.Add( m.Value.Trim( '\u0000' ) );
            }
        }
    }
}
