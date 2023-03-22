namespace VfxEditor.Select.Scd.BgmQuest {
    public class BgmQuestRow {
        public readonly string Name;
        public readonly int RowId;
        public readonly ushort BgmId;

        public BgmQuestRow( Lumina.Excel.GeneratedSheets.BGMSwitch bgm ) {
            Name = bgm.Quest.Value?.Name.ToString();
            RowId = ( int )bgm.RowId;
            BgmId = bgm.BGM;
        }
    }
}
