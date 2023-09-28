using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tmb.Emote {
    public class EmoteRow {
        public readonly ushort Icon;
        public readonly int RowId;
        public readonly string Name;
        public readonly List<string> TmbFiles;

        public EmoteRow( Lumina.Excel.GeneratedSheets.Emote emote ) {
            RowId = ( int )emote.RowId;
            Icon = emote.Icon;
            Name = emote.Name.ToString();
            TmbFiles = emote.ActionTimeline.Select( x => x?.Value?.Key.ToString() )
                .Where( x => !string.IsNullOrEmpty( x ) ).Select( SelectDataUtils.ToTmbPath ).ToList();
        }
    }
}
