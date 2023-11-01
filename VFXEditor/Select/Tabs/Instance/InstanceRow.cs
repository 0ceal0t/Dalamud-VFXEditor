using Lumina.Excel.GeneratedSheets;

namespace VfxEditor.Select.Tabs.Instance {
    public class InstanceRow {
        public readonly string Name;
        public readonly int RowId;
        public readonly byte ContentType;
        public readonly uint Image;
        public readonly ushort ContentRowId;

        public InstanceRow( ContentFinderCondition content ) {
            Name = content.Name.ToString();
            RowId = ( int )content.RowId;
            Image = content.Image;
            ContentType = content.ContentLinkType;
            ContentRowId = content.Content; // only = 1 is ok
        }
    }
}