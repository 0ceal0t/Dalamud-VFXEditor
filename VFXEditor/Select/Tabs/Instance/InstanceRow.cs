using Lumina.Excel.GeneratedSheets2;
using VfxEditor.Select.Base;

namespace VfxEditor.Select.Tabs.Instance {
    public class InstanceRow : ISelectItem {
        public readonly string Name;
        public readonly int RowId;
        public readonly byte ContentType;
        public readonly uint Image;
        public readonly uint ContentRowId;

        public InstanceRow( ContentFinderCondition content ) {
            Name = content.Name.ToString();
            RowId = ( int )content.RowId;
            Image = content.Image;
            ContentType = content.ContentLinkType;
            ContentRowId = content.Content.Row; // only = 1 is ok
        }

        public string GetName() => Name;
    }
}