using System.Linq;

namespace VfxEditor.Select.Tmb.Action {
    public class NonPlayerActionTab : ActionTab {
        public NonPlayerActionTab( SelectDialog dialog, string name ) : base( dialog, name, "Tmb-NonPlayerAction" ) { }

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && !( x.IsPlayerAction || x.ClassJob.Value != null ) && !x.AffectsPosition );
            foreach( var item in sheet ) Items.Add( new ActionRow( item ) );
        }
    }
}
