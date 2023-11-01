using Lumina.Excel.GeneratedSheets;
using System.Linq;

namespace VfxEditor.Select.Tabs.Statuses {
    public class StatusTabVfx : SelectTab<StatusRow> {
        public StatusTabVfx( SelectDialog dialog, string name ) : base( dialog, name, "Status-Vfx", SelectResultType.GameStatus ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Status>().Where( x => !string.IsNullOrEmpty( x.Name ) );
            foreach( var item in sheet ) {
                var status = new StatusRow( item );
                if( status.VfxExists ) Items.Add( status );
            }
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            DrawIcon( Selected.Icon );
            DrawPath( "Hit", Selected.HitPath, $"{Selected.Name} Hit", true );
            DrawPath( "Loop 1", Selected.LoopPath1, $"{Selected.Name} Loop 1", true );
            DrawPath( "Loop 2", Selected.LoopPath2, $"{Selected.Name} Loop 2", true );
            DrawPath( "Loop 3", Selected.LoopPath3, $"{Selected.Name} Loop 3", true );
        }

        protected override string GetName( StatusRow item ) => item.Name;
    }
}
