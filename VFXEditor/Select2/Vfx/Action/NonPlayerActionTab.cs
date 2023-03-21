using System;
using System.Linq;

namespace VfxEditor.Select2.Vfx.Action {
    public class NonPlayerActionTab : ActionTab {
        public NonPlayerActionTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        public override void OnLoad() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && !( x.IsPlayerAction || x.ClassJob.Value != null ) );

            foreach( var item in sheet ) {
                var action = new ActionRow( item, false );
                if( action.HasVfx ) Items.Add( action );
                if( action.HitAction != null ) Items.Add( action.HitAction );
            }
        }
    }
}
