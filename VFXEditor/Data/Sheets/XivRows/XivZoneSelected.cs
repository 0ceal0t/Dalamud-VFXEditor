using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VFXEditor {
    public class XivZoneSelected {

        public XivZone Zone;
        public List<string> VfxPaths = new List<string>();

        public static Regex rx = new Regex( @"\u0000([a-zA-Z0-9\/_]*?)\.avfx", RegexOptions.Compiled );

        public XivZoneSelected( Lumina.Data.Files.LgbFile file, XivZone zone ) {
            Zone = zone;

            if( file != null ) {
                var data = file.Data;

                string stringData = Encoding.UTF8.GetString( data );
                MatchCollection matches = rx.Matches( stringData );
                foreach( Match m in matches ) {
                    VfxPaths.Add( m.Value.Trim( '\u0000') );
                }
            }
        }
    }
}
