using System.Linq;

namespace VfxEditor.Select.Vfx.Action {
    public class NonPlayerActionTab : ActionTab {
        public NonPlayerActionTab( SelectDialog dialog, string name ) : base( dialog, name, "Vfx-NonPlayerAction" ) { }

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && !( x.IsPlayerAction || x.ClassJob.Value != null ) );
            foreach( var item in sheet ) {
                var action = new ActionRow( item, false );
                Items.Add( action );
                if( action.HitAction != null ) Items.Add( action.HitAction );
            }
        }
    }
}
