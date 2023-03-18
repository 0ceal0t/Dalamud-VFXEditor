using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VfxEditor.Select.Rows {
    public class XivGimmickSelected {
        public XivGimmick Gimmick;
        public List<string> VfxPaths = new();
        public string TmbPath;
        public bool VfxExists = false;

        public XivGimmickSelected( Lumina.Data.FileResource file, XivGimmick gimmick ) {
            Gimmick = gimmick;

            if( file != null ) {
                var data = file.Data;
                TmbPath = file.FilePath.Path;
                VfxExists = true;

                var stringData = Encoding.UTF8.GetString( data );
                var matches = SheetManager.AvfxRegex.Matches( stringData );
                foreach( Match m in matches ) {
                    VfxPaths.Add( m.Value.Trim( '\u0000' ) );
                }
            }
        }
    }
}
