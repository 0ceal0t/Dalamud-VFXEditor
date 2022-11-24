using System.Collections.Generic;

namespace VfxEditor.Select.Rows {
    public class XivGimmick {
        public string Name;
        public int RowId;
        public string TmbPath;

        public XivGimmick( Lumina.Excel.GeneratedSheets.ActionTimeline timeline, Dictionary<string, string> suffixToName ) {
            RowId = ( int )timeline.RowId;
            Name = timeline.Key.ToString();
            TmbPath = $"chara/action/{Name}.tmb";
            Name = Name.Replace( "mon_sp/gimmick/", "" );

            var split = Name.Split( '_' );
            if( split.Length > 0 ) {
                var suffix = split[0];
                if( suffixToName.ContainsKey( suffix ) ) {
                    Name = "(" + suffixToName[suffix] + ") " + Name;
                }
            }
        }
    }
}
