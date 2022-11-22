using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VfxEditor.Select.Rows {
    public class XivZoneSelected {
        public readonly XivZone Zone;
        public readonly List<string> VfxPaths = new();

        private static readonly Regex rx = new( @"\u0000([a-zA-Z0-9\/_]*?)\.avfx", RegexOptions.Compiled );

        public XivZoneSelected( Lumina.Data.Files.LgbFile file, XivZone zone ) {
            Zone = zone;

            if( file != null ) {
                var data = file.Data;

                var stringData = Encoding.UTF8.GetString( data );
                var matches = rx.Matches( stringData );
                foreach( var m in matches.Cast<Match>() ) {
                    VfxPaths.Add( m.Value.Trim( '\u0000' ) );
                }
            }
        }
    }
}
