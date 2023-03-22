using Lumina.Excel.GeneratedSheets;
using System.Linq;

namespace VfxEditor.Select.Scd.Bgm {
    public class BgmTab : SelectTab<BgmRow> {
        public BgmTab( SelectDialog dialog, string name ) : base( dialog, name, "Scd-Bgm" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<BGM>().Where( x => !string.IsNullOrEmpty( x.File ) );

            foreach( var item in sheet ) Items.Add( new BgmRow( item ) );
        }

        // ===== DRAWING ======

        protected override void DrawSelected( string parentId ) {
            Dialog.DrawPath( "Path", Selected.Path, parentId, SelectResultType.GameMusic, Selected.Name );
        }

        protected override string GetName( BgmRow item ) => item.Name;
    }
}
