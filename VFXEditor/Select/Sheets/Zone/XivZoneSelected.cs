using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VfxEditor.Select.Rows {
    public class XivZoneSelected {
        public readonly XivZone Zone;
        public readonly List<string> VfxPaths = new();

        public XivZoneSelected( Lumina.Data.Files.LgbFile file, XivZone zone ) {
            Zone = zone;

            if( file != null ) {
                var data = file.Data;

                var stringData = Encoding.UTF8.GetString( data );
                var matches = SheetManager.AvfxRegex.Matches( stringData );
                foreach( var m in matches.Cast<Match>() ) {
                    VfxPaths.Add( m.Value.Trim( '\u0000' ) );
                }
            }
        }
    }
}
