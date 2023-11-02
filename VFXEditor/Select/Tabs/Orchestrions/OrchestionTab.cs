using Lumina.Excel.GeneratedSheets;
using System.Linq;

namespace VfxEditor.Select.Tabs.Orchestrions {
    public class SelectedOrchestrion {
        public readonly string Path;

        public SelectedOrchestrion( OrchestrionRow orchestrion ) {
            var pathRow = Dalamud.DataManager.GetExcelSheet<OrchestrionPath>().GetRow( ( uint )orchestrion.RowId );
            Path = pathRow.File.ToString();
        }
    }

    public class OrchestrionTab : SelectTab<OrchestrionRow, SelectedOrchestrion> {
        public OrchestrionTab( SelectDialog dialog, string name ) : base( dialog, name, "Orchestrion", SelectResultType.GameMusic ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Orchestrion>().Where( x => !string.IsNullOrEmpty( x.Name ) );
            foreach( var item in sheet ) Items.Add( new OrchestrionRow( item ) );
        }

        public override void LoadSelection( OrchestrionRow item, out SelectedOrchestrion loaded ) {
            loaded = new( item );
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            DrawPath( "Path", Loaded.Path, Selected.Name );
        }

        protected override string GetName( OrchestrionRow item ) => item.Name;
    }
}