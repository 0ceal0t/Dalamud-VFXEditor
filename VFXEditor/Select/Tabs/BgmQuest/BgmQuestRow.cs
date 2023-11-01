using Lumina.Excel.GeneratedSheets;

namespace VfxEditor.Select.Tabs.BgmQuest {
    public class BgmQuestRow {
        public readonly string Name;
        public readonly int RowId;
        public readonly ushort BgmId;

        public BgmQuestRow( BGMSwitch bgm ) {
            Name = bgm.Quest.Value?.Name.ToString();
            RowId = ( int )bgm.RowId;
            BgmId = bgm.BGM;
        }
    }
}