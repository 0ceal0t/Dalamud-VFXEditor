using Lumina.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VfxEditor.Select.Rows {
    public partial class XivActionSelected {
        public readonly XivAction Action;
        public readonly string CastVfxPath;
        public readonly string SelfTmbPath;
        public readonly List<string> SelfVfxPaths = new();

        public XivActionSelected( FileResource file, XivAction action ) {
            Action = action;
            CastVfxPath = action.CastVfxPath;

            if( file == null ) return;
            var data = file.Data;
            SelfTmbPath = file.FilePath.Path;

            var matches = SheetManager.AvfxRegex.Matches( Encoding.UTF8.GetString( data ) );
            foreach( var m in matches.Cast<Match>() ) SelfVfxPaths.Add( m.Value.Trim( '\u0000' ) );
        }
    }
}
