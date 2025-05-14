using Lumina.Excel.Sheets;
using System.Linq;

namespace VfxEditor.Select.Tabs.Actions {
    public class ActionTabPapNonPlayer : ActionTabPap {
        public ActionTabPapNonPlayer( SelectDialog dialog, string name ) : base( dialog, name, "Action-Pap-NonPlayer" ) { }

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name.ExtractText() ) && !( x.IsPlayerAction || x.ClassJob.ValueNullable != null ) ); //&& !x.AffectsPosition );
            foreach( var item in sheet ) Items.Add( new( item ) );
        }
    }
}