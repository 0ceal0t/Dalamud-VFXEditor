using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VFXSelect.Data.Rows {
    public class XivEmoteSelected {
        public XivEmote Emote;

        public HashSet<string> VfxPaths = new();

        public static readonly Regex rx = new( @"\u0000([a-zA-Z0-9\/_]*?)\.avfx", RegexOptions.Compiled );

        public XivEmoteSelected( XivEmote emote, List<Lumina.Data.FileResource> files ) {
            Emote = emote;

            foreach( var f in files ) {
                var data = f.Data;
                var stringData = Encoding.UTF8.GetString( data );
                var matches = rx.Matches( stringData );
                foreach( Match m in matches ) {
                    VfxPaths.Add( m.Value.Trim( '\u0000' ) );
                }
            }
        }
    }
}
