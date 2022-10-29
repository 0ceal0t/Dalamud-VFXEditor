using System.Collections.Generic;

namespace VfxEditor.Select.Rows {
    public class XivEmote {
        public string Name;
        public int RowId;
        public ushort Icon;
        public List<string> PapFiles = new();

        public static readonly string RootPath = "chara/human/c1101/animation/a0001/bt_common/";

        public XivEmote( Lumina.Excel.GeneratedSheets.Emote emote ) {
            RowId = ( int )emote.RowId;
            Name = emote.Name;
            Icon = emote.Icon;

            // chara/human/c1101/animation/a0001/bt_common/emote_sp/sp08.pap
            foreach( var e in emote.ActionTimeline ) {
                var emoteKey = e.Value?.Key.ToString();
                if( !string.IsNullOrEmpty( emoteKey ) && emoteKey.Contains( "emote_sp" ) ) {
                    PapFiles.Add( RootPath + emoteKey + ".pap" );
                }
            }
        }
    }
}
