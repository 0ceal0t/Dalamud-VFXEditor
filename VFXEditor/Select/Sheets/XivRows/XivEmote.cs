using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXSelect.Data.Rows {
    public class XivEmote {
        public string Name;
        public int RowId;
        public ushort Icon;
        public List<string> PapFiles = new List<string>();

        public static string RootPath = "chara/human/c1101/animation/a0001/bt_common/";

        public XivEmote( Lumina.Excel.GeneratedSheets.Emote emote) {
            RowId = ( int )emote.RowId;
            Name = emote.Name;
            Icon = emote.Icon;
            // "chara/human/c1101/animation/a0001/bt_common/emote_sp/sp08.pap"
            var emoteKeys = emote.ActionTimeline;
            foreach(var e in emoteKeys ) {
                string emoteKey = e.Value?.Key.ToString();
                if(!string.IsNullOrEmpty(emoteKey) && emoteKey.Contains( "emote_sp" ) ) {
                    PapFiles.Add( RootPath + emoteKey + ".pap" );
                }
            }
        }
    }
}
