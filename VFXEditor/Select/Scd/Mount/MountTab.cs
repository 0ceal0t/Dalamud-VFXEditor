using System.Linq;
using VfxEditor.Select.Shared.Mount;

namespace VfxEditor.Select.Scd.Mount {
    public class MountTab : SelectTab<MountRow> {
        public MountTab( SelectDialog dialog, string name ) : base( dialog, name, "Shared-Mount" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Mount>().Where( x => !string.IsNullOrEmpty( x.Singular ) );

            foreach( var item in sheet ) Items.Add( new MountRow( item ) );
        }

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        protected override void DrawSelected( string parentId ) {
            SelectTabUtils.DrawIcon( Icon );

            Dialog.DrawPath( "Bgm", Selected.Bgm, parentId, SelectResultType.GameNpc, Selected.Name );
        }

        protected override string GetName( MountRow item ) => item.Name;
    }
}
