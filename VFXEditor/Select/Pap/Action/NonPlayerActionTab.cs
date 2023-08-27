using System.Linq;

namespace VfxEditor.Select.Pap.Action {
    public class NonPlayerActionTab : ActionTab {
        public NonPlayerActionTab( SelectDialog dialog, string name ) : base( dialog, name, "Pap-NonPlayerAction" ) { }

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && !( x.IsPlayerAction || x.ClassJob.Value != null ) && !x.AffectsPosition );
            foreach( var item in sheet ) Items.Add( new( item ) );
        }
    }
}
