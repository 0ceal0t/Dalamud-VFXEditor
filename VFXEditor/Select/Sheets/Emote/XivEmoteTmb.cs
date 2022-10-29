using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Rows {
    public class XivEmoteTmb {
        public readonly ushort Icon;
        public readonly int RowId;
        public readonly string Name;

        public readonly List<string> TmbFiles;

        public XivEmoteTmb( Lumina.Excel.GeneratedSheets.Emote emote ) {
            RowId = ( int )emote.RowId;
            Icon = emote.Icon;
            Name = emote.Name.ToString();

            TmbFiles = emote.ActionTimeline.Select( x => x?.Value?.Key.ToString() ).Where( x => !string.IsNullOrEmpty( x ) ).Select( x => ToTmb( x ) ).ToList();
        }

        private static string ToTmb( string key ) {
            if( string.IsNullOrEmpty( key ) ) return "";
            return $"chara/action/{key}.tmb";
        }
    }
}
