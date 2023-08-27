using System.Linq;

namespace VfxEditor.Select.Shared.Mount {
    public abstract class MountTab : SelectTab<MountRow> {
        public MountTab( SelectDialog dialog, string name ) : base( dialog, name, "Shared-Mount", SelectResultType.GameMount ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Mount>().Where( x => !string.IsNullOrEmpty( x.Singular ) );
            foreach( var item in sheet ) Items.Add( new MountRow( item ) );
        }

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        protected override string GetName( MountRow item ) => item.Name;
    }
}
