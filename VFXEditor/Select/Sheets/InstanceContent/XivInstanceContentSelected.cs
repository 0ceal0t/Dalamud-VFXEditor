using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.Select.Rows {
    public class XivInstanceContentSelected {
        public readonly XivInstanceContent Content;
        public readonly BgmSituationStruct Situation;

        public XivInstanceContentSelected( XivInstanceContent content ) {
            Content = content;
            var instance = Plugin.DataManager.GetExcelSheet<InstanceContent>().GetRow( content.ContentRowId );
            Situation = XivBgmQuestSelected.GetBgmSituation( ( ushort )instance.BGM.Row );
        }
    }
}
