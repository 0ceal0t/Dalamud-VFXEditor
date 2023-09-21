using Lumina.Excel.GeneratedSheets;
using System.Linq;

namespace VfxEditor.Select.Scd.Bgm {
    public class BgmTab : SelectTab<BgmRow> {
        public BgmTab( SelectDialog dialog, string name ) : base( dialog, name, "Scd-Bgm", SelectResultType.GameMusic ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<BGM>().Where( x => !string.IsNullOrEmpty( x.File ) );
            foreach( var item in sheet ) Items.Add( new BgmRow( item ) );
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            DrawPath( "Path", Selected.Path, Selected.Name );
        }

        protected override string GetName( BgmRow item ) => item.Name;
    }
}
