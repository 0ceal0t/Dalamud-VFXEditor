using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
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
            DrawPaths( new Dictionary<string, string>() {
                { "Hit", Selected.HitPath },
                { "Loop 1", Selected.LoopPath1 },
                { "Loop 2", Selected.LoopPath2 },
                { "Loop 3", Selected.LoopPath3 },
            }, Selected.Name );
        }

        protected override string GetName( StatusRow item ) => item.Name;

        protected override uint GetIconId( StatusRow item ) => item.Icon;
    }
}
