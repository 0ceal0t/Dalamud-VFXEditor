using Lumina.Excel.GeneratedSheets2;
using System.Linq;

namespace VfxEditor.Select.Tabs.Mounts {
    public abstract class MountTab<T> : SelectTab<MountRow, T> where T : class {
        public MountTab( SelectDialog dialog, string name ) : base( dialog, name, "Mount" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Mount>().Where( x => !string.IsNullOrEmpty( x.Singular ) );
            foreach( var item in sheet ) Items.Add( new MountRow( item ) );
        }
    }
}