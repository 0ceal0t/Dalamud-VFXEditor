namespace VfxEditor.Select.Tex.Action {
    public class ActionRow {
        public readonly string Name;
        public readonly int RowId;
        public readonly ushort Icon;

        public ActionRow( Lumina.Excel.GeneratedSheets.Action action ) {
            Name = action.Name.ToString();
            RowId = ( int )action.RowId;
            Icon = action.Icon;
        }
    }
}
