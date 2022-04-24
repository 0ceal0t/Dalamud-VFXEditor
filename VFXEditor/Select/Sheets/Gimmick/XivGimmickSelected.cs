using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VFXSelect.Select.Rows {
    public class XivGimmickSelected {
        public XivGimmick Gimmick;
        public List<string> VfxPaths = new();
        public string TmbPath;
        public bool VfxExists = false;

        public static readonly Regex rx = new( @"\u0000([a-zA-Z0-9\/_]*?)\.avfx", RegexOptions.Compiled );

        public XivGimmickSelected( Lumina.Data.FileResource file, XivGimmick gimmick ) {
            Gimmick = gimmick;

            if( file != null ) {
                var data = file.Data;
                TmbPath = file.FilePath.Path;
                VfxExists = true;

                var stringData = Encoding.UTF8.GetString( data );
                var matches = rx.Matches( stringData );
                foreach( Match m in matches ) {
                    VfxPaths.Add( m.Value.Trim( '\u0000' ) );
                }
            }
        }
    }
}
