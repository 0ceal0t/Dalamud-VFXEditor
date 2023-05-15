using System.Collections.Generic;

namespace VfxEditor.Select.Vfx.Gimmick {
    public class GimmickRow {
        public readonly string Name;
        public readonly int RowId;
        public readonly string TmbPath;

        public GimmickRow( Lumina.Excel.GeneratedSheets.ActionTimeline timeline, Dictionary<string, string> suffixToName ) {
            RowId = ( int )timeline.RowId;
            Name = timeline.Key.ToString();
            TmbPath = $"chara/action/{Name}.tmb";
            Name = Name.Replace( "mon_sp/gimmick/", "" );

            var split = Name.Split( '_' );
            if( split.Length > 0 ) {
                var suffix = split[0];
                if( suffixToName.TryGetValue( suffix, out var value ) ) {
                    Name = "(" + value + ") " + Name;
                }
            }
        }
    }
}
