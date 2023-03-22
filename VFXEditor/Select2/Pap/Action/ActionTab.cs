using System.Linq;

namespace VfxEditor.Select2.Pap.Action {
    public class ActionTab : SelectTab<ActionRow, ActionRowSelected> {
        public ActionTab( SelectDialog dialog, string name, string stateId ) : base( dialog, name, stateId ) { }
        public ActionTab( SelectDialog dialog, string name ) : base( dialog, name, "Pap-Action" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && ( x.IsPlayerAction || x.ClassJob.Value != null ) && !x.AffectsPosition );

            foreach( var item in sheet ) Items.Add( new ActionRow( item ) );
        }

        public override void LoadSelection( ActionRow item, out ActionRowSelected loaded ) {
            loaded = new (
                SelectUtils.FileExistsFilter( SelectUtils.GetAllSkeletonPaths( item.Start ) ),
                SelectUtils.FileExistsFilter( SelectUtils.GetAllSkeletonPaths( item.End ) ),
                SelectUtils.FileExistsFilter( SelectUtils.GetAllSkeletonPaths( item.Hit ) )
            );
        }

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        protected override void DrawSelected( string parentId ) {
            SelectTabUtils.DrawIcon( Icon );

            Dialog.DrawPapDict( Loaded.StartAnimations, "Start", Selected.Name, $"{parentId}/Start" );
            Dialog.DrawPapDict( Loaded.EndAnimations, "End", Selected.Name, $"{parentId}/End" );
            Dialog.DrawPapDict( Loaded.HitAnimations, "Hit", Selected.Name, $"{parentId}/Hit" );
        }

        protected override string GetName( ActionRow item ) => item.Name;
    }
}
