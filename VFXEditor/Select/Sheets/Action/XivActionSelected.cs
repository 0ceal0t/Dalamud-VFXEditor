using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VfxEditor.Select.Rows {
    public class XivActionSelected {
        public XivActionBase Action;
        public string CastVfxPath;
        public string SelfTmbPath;
        public List<string> SelfVfxPaths = new();

        public static readonly Regex Rx = new( @"\u0000([a-zA-Z0-9\/_]*?)\.avfx", RegexOptions.Compiled );

        public XivActionSelected( Lumina.Data.FileResource file, XivActionBase action ) {
            Action = action;
            CastVfxPath = action.CastVfxPath;

            if( file == null ) return;
            var data = file.Data;
            SelfTmbPath = file.FilePath.Path;

            var stringData = Encoding.UTF8.GetString( data );
            var matches = Rx.Matches( stringData );
            foreach( Match m in matches ) SelfVfxPaths.Add( m.Value.Trim( '\u0000' ) );
        }
    }
}
