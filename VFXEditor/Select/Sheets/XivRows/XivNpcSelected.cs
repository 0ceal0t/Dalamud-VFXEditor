using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VFXSelect.Data.Rows {
    public class XivNpcSelected {
        public XivNpc Npc;
        public int Count;
        public string ImcPath;

        public HashSet<string> VfxPaths = new HashSet<string>();

        public static Regex rx = new Regex( @"\u0000([a-zA-Z0-9\/_]*?)\.avfx", RegexOptions.Compiled );

        public XivNpcSelected( Lumina.Data.Files.ImcFile file, XivNpc npc, List<Lumina.Data.FileResource> files ) {
            Npc = npc;
            Count = file.Count;
            ImcPath = file.FilePath;

            foreach(var p in file.GetParts() ) {
                var id = p.Variants[npc.Variant - 1].VfxId;
                if(id != 0 ) {
                    VfxPaths.Add( npc.GetVfxPath( id ) );
                }
            }

            foreach(var f in files ) {
                var data = f.Data;
                string stringData = Encoding.UTF8.GetString( data );
                MatchCollection matches = rx.Matches( stringData );
                foreach( Match m in matches ) {
                    VfxPaths.Add( m.Value.Trim( '\u0000') );
                }
            }
        }
    }
}
