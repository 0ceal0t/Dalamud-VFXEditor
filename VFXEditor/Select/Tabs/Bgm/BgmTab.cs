using Lumina.Excel.GeneratedSheets;
using System.Linq;

namespace VfxEditor.Select.Tabs.Bgm {
    public class BgmTab : SelectTab<BgmRow> {
        public BgmTab( SelectDialog dialog, string name ) : base( dialog, name, "Bgm", SelectResultType.GameMusic ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<BGM>().Where( x => !string.IsNullOrEmpty( x.File ) );
            foreach( var item in sheet ) Items.Add( new BgmRow( item ) );
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            DrawPaths( Selected.Path, Selected.Name );
        }

        protected override string GetName( BgmRow item ) => item.Name;
    }
}