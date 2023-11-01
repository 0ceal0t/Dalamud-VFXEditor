using Lumina.Excel.GeneratedSheets;
using System.Linq;

namespace VfxEditor.Select.Tabs.Actions {
    public class ActionTabTmbNonPlayer : ActionTabTmb {
        public ActionTabTmbNonPlayer( SelectDialog dialog, string name ) : base( dialog, name, "Action-Tmb-NonPlayer" ) { }

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && !( x.IsPlayerAction || x.ClassJob.Value != null ) && !x.AffectsPosition );
            foreach( var item in sheet ) Items.Add( new ActionRow( item ) );
        }
    }
}