using System.Collections.Generic;

namespace VFXSelect.Select.Rows {
    public class XivNpc : XivNpcBase {
        public bool CSV_Defined = false;

        public XivNpc( Lumina.Excel.GeneratedSheets.ModelChara npc, Dictionary<int, string> NpcIdToName ) : base( npc ) {
            CSV_Defined = NpcIdToName.ContainsKey( RowId );
            if( CSV_Defined ) {
                Name = NpcIdToName[RowId];
            }
        }
    }
}
