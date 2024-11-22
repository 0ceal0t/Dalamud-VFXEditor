using Lumina.Excel.Sheets;
using VfxEditor.Select.Base;

namespace VfxEditor.Select.Tabs.BgmQuest {
    public class BgmQuestRow : ISelectItem {
        public readonly string Name;
        public readonly int RowId;
        public readonly uint BgmId;

        public BgmQuestRow( BGMSwitch bgm ) {
            Name = bgm.Quest.ValueNullable?.Name.ToString();
            RowId = ( int )bgm.RowId;
            BgmId = bgm.BGM.RowId;
        }

        public string GetName() => Name;
    }
}