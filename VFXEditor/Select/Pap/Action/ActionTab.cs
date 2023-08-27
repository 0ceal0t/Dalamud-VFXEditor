using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Pap.Action {
    public class ActionTab : SelectTab<ActionRow, Dictionary<string, Dictionary<string, string>>> {
        public ActionTab( SelectDialog dialog, string name ) : this( dialog, name, "Pap-Action" ) { }

        public ActionTab( SelectDialog dialog, string name, string stateId ) : base( dialog, name, stateId, SelectResultType.GameAction ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && ( x.IsPlayerAction || x.ClassJob.Value != null ) && !x.AffectsPosition );
            foreach( var item in sheet ) Items.Add( new ActionRow( item ) );
        }

        public override void LoadSelection( ActionRow item, out Dictionary<string, Dictionary<string, string>> loaded ) {
            loaded = new Dictionary<string, Dictionary<string, string>> {
                { "Start", SelectDataUtils.FileExistsFilter( SelectDataUtils.GetAllSkeletonPaths( item.Start ) ) },
                { "End", SelectDataUtils.FileExistsFilter( SelectDataUtils.GetAllSkeletonPaths( item.End ) ) },
                { "Hit", SelectDataUtils.FileExistsFilter( SelectDataUtils.GetAllSkeletonPaths( item.Hit ) ) }
            };
        }

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        protected override void DrawSelected() {
            SelectUiUtils.DrawIcon( Icon );

            DrawPapsWithHeader( Loaded, Selected.Name );
        }

        protected override string GetName( ActionRow item ) => item.Name;
    }
}
