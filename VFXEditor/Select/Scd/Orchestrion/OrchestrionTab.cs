using System.Linq;

namespace VfxEditor.Select.Scd.Orchestrion {
    public class OrchestrionTab : SelectTab<OrchestrionRow, OrchestionRowSelected> {
        public OrchestrionTab( SelectDialog dialog, string name ) : base( dialog, name, "Scd-Orchestrion" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Orchestrion>().Where( x => !string.IsNullOrEmpty( x.Name ) );
            foreach( var item in sheet ) Items.Add( new OrchestrionRow( item ) );
        }

        public override void LoadSelection( OrchestrionRow item, out OrchestionRowSelected loaded ) {
            loaded = new( item );
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            Dialog.DrawPath( "Path", Loaded.Path, SelectResultType.GameMusic, Selected.Name );
        }

        protected override string GetName( OrchestrionRow item ) => item.Name;
    }
}
