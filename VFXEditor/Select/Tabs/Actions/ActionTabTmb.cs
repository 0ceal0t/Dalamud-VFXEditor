using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Actions {
    public class ActionTabTmb : SelectTab<ActionRow> {
        public ActionTabTmb( SelectDialog dialog, string name ) : this( dialog, name, "Action-Tmb" ) { }

        public ActionTabTmb( SelectDialog dialog, string name, string stateId ) : base( dialog, name, stateId ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && ( x.IsPlayerAction || x.ClassJob.Value != null ) && !x.AffectsPosition );
            foreach( var item in sheet ) Items.Add( new ActionRow( item ) );
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            Dialog.DrawPaths( new Dictionary<string, string>() {
                { "Start",  Selected.StartPath },
                { "End",  Selected.EndPath },
                { "Hit",  Selected.HitPath },
                { "Weapon",  Selected.WeaponPath },

            }, Selected.Name, SelectResultType.GameAction );
        }
    }
}