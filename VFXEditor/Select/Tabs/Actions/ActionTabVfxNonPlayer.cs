using Lumina.Excel.GeneratedSheets2;
using System.Linq;

namespace VfxEditor.Select.Tabs.Actions {
    public class ActionTabVfxNonPlayer : ActionTabVfx {
        public ActionTabVfxNonPlayer( SelectDialog dialog, string name ) : base( dialog, name, "Action-Vfx-NonPlayer" ) { }

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && !( x.IsPlayerAction || x.ClassJob.Value != null ) );

            foreach( var item in sheet ) {
                var action = new ActionRowVfx( item );
                Items.Add( action );
                if( action.HitAction != null ) Items.Add( action.HitAction );
            }
        }
    }
}