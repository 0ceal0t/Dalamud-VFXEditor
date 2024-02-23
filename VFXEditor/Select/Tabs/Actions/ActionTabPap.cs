using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Actions {
    public class ActionTabPap : SelectTab<ActionRowPap, Dictionary<string, Dictionary<string, string>>> {
        public ActionTabPap( SelectDialog dialog, string name ) : this( dialog, name, "Action-Pap" ) { }

        public ActionTabPap( SelectDialog dialog, string name, string stateId ) : base( dialog, name, stateId, SelectResultType.GameAction ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && ( x.IsPlayerAction || x.ClassJob.Value != null ) && !x.AffectsPosition );
            foreach( var item in sheet ) Items.Add( new ActionRowPap( item ) );
        }

        public override void LoadSelection( ActionRowPap item, out Dictionary<string, Dictionary<string, string>> loaded ) {
            loaded = new Dictionary<string, Dictionary<string, string>> {
                { "Start", SelectDataUtils.FileExistsFilter( SelectDataUtils.GetAllSkeletonPaths( item.Start ) ) },
                { "End", SelectDataUtils.FileExistsFilter( SelectDataUtils.GetAllSkeletonPaths( item.End ) ) },
                { "Hit", SelectDataUtils.FileExistsFilter( SelectDataUtils.GetAllSkeletonPaths( item.Hit ) ) }
            };
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            DrawIcon( Selected.Icon );
            DrawPaths( Loaded, Selected.Name );
        }

        protected override string GetName( ActionRowPap item ) => item.Name;
    }
}