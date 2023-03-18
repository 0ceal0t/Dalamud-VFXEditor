using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VfxEditor.Select.Rows {
    public partial class XivEmoteSelected {
        public readonly XivEmote Emote;
        public readonly HashSet<string> VfxPaths = new();

        public XivEmoteSelected( XivEmote emote, List<Lumina.Data.FileResource> files ) {
            Emote = emote;

            foreach( var f in files ) {
                var data = f.Data;
                var stringData = Encoding.UTF8.GetString( data );
                var matches = SheetManager.AvfxRegex.Matches( stringData );
                foreach( Match m in matches ) {
                    VfxPaths.Add( m.Value.Trim( '\u0000' ) );
                }
            }
        }
    }
}
