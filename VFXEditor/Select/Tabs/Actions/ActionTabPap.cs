using Lumina.Excel.Sheets;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Actions {
    public class ActionTabPap : SelectTab<ActionRowPap, Dictionary<string, Dictionary<string, string>>> {
        public ActionTabPap( SelectDialog dialog, string name ) : this( dialog, name, "Action-Pap" ) { }

        public ActionTabPap( SelectDialog dialog, string name, string stateId ) : base( dialog, name, stateId ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name.ExtractText() ) && ( x.IsPlayerAction || x.ClassJob.ValueNullable != null ) ); //&& !x.AffectsPosition );
            foreach( var item in sheet ) Items.Add( new ActionRowPap( item ) );
        }

        public override void LoadSelection( ActionRowPap item, out Dictionary<string, Dictionary<string, string>> loaded ) {
            loaded = new Dictionary<string, Dictionary<string, string>> {
                { "Start", SelectDataUtils.FileExistsFilter( SelectDataUtils.GetAllSkeletonPaths( item.StartPath ) ) },
                { "End", SelectDataUtils.FileExistsFilter( SelectDataUtils.GetAllSkeletonPaths( item.EndPath ) ) },
                { "Hit", SelectDataUtils.FileExistsFilter( SelectDataUtils.GetAllSkeletonPaths( item.HitPath ) ) }
            };
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            Dialog.DrawPaths( Loaded, Selected.Name, SelectResultType.GameAction );
        }
    }
}