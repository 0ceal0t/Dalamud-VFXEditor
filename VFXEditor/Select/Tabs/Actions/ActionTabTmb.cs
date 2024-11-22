using Lumina.Excel.Sheets;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Actions {
    public class ActionTabTmb : SelectTab<ActionRow> {
        public ActionTabTmb( SelectDialog dialog, string name ) : this( dialog, name, "Action-Tmb" ) { }

        public ActionTabTmb( SelectDialog dialog, string name, string stateId ) : base( dialog, name, stateId ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name.ExtractText() ) && ( x.IsPlayerAction || x.ClassJob.ValueNullable != null ) && !x.AffectsPosition );
            foreach( var item in sheet ) Items.Add( new ActionRow( item ) );
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            Dialog.DrawPaths( new Dictionary<string, string>() {
                { "Start",  Selected.StartTmbPath },
                { "End",  Selected.EndTmbPath },
                { "Hit",  Selected.HitTmbPath },
                { "Weapon",  Selected.WeaponTmbPath },

            }, Selected.Name, SelectResultType.GameAction );
        }
    }
}