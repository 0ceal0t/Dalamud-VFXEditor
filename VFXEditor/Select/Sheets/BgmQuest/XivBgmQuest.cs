namespace VfxEditor.Select.Rows {
    public class XivBgmQuest {
        public readonly string Name;
        public readonly int RowId;
        public ushort BgmId;

        public XivBgmQuest( Lumina.Excel.GeneratedSheets.BGMSwitch bgm ) {
            Name = bgm.Quest.Value?.Name.ToString();
            RowId = ( int )bgm.RowId;
            BgmId = bgm.BGM;
        }
    }
}
