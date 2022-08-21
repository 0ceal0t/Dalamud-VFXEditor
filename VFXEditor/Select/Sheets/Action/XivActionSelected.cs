using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VFXEditor.Select.Rows {
    public class XivActionSelected {
        public XivActionBase Action;
        public bool CastVfxExists = false;
        public string CastVfxPath;

        public bool SelfVfxExists = false;
        public string SelfTmbPath;
        public List<string> SelfVfxPaths = new();

        public static readonly Regex rx = new( @"\u0000([a-zA-Z0-9\/_]*?)\.avfx", RegexOptions.Compiled );

        public XivActionSelected( Lumina.Data.FileResource file, XivActionBase action ) {
            Action = action;
            CastVfxExists = action.CastKeyExists;
            CastVfxPath = action.GetCastVFXPath();

            if( file != null ) {
                var data = file.Data;
                SelfVfxExists = true;
                SelfTmbPath = file.FilePath.Path;

                var stringData = Encoding.UTF8.GetString( data );
                var matches = rx.Matches( stringData );
                foreach( Match m in matches ) {
                    SelfVfxPaths.Add( m.Value.Trim( '\u0000' ) );
                }
            }
        }
    }
}
