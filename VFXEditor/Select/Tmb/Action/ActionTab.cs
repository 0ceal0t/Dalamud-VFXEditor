using Dalamud.Interface;
using ImGuiNET;
using System.Linq;
using VfxEditor.Utils;

namespace VfxEditor.Select.Tmb.Action {
    public class ActionTab : SelectTab<ActionRow> {
        public ActionTab( SelectDialog dialog, string name, string stateId ) : base( dialog, name, stateId ) { }
        public ActionTab( SelectDialog dialog, string name ) : base( dialog, name, "Tmb-Action" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && ( x.IsPlayerAction || x.ClassJob.Value != null ) && !x.AffectsPosition );

            foreach( var item in sheet ) Items.Add( new ActionRow( item ) );
        }

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        protected override void DrawSelected( string parentId ) {
            SelectTabUtils.DrawIcon( Icon );

            Dialog.DrawPath( "Start", Selected.Start.Path, $"{parentId}/Start", SelectResultType.GameAction, $"{Selected.Name} Start", true );
            DrawMovementCancel( Selected.Start );

            Dialog.DrawPath( "End", Selected.End.Path, $"{parentId}/End", SelectResultType.GameAction, $"{Selected.Name} End", true );
            DrawMovementCancel( Selected.End );

            Dialog.DrawPath( "Hit", Selected.Hit.Path, $"{parentId}/Hit", SelectResultType.GameAction, $"{Selected.Name} Hit", true );
            Dialog.DrawPath( "Weapon", Selected.Weapon.Path, $"{parentId}/Weapon", SelectResultType.GameAction, $"{Selected.Name} Weapon", true );
        }

        protected override string GetName( ActionRow item ) => item.Name;

        private void DrawMovementCancel( ActionTmbData data ) {
            if( !data.IsMotionDisabled ) return;
            if( Dialog.IsSource ) return;
            ImGui.Indent( 25f );
            UiUtils.IconText( FontAwesomeIcon.QuestionCircle, true );
            UiUtils.Tooltip( "This parameter is set in the game's Excel sheet, and cannot be removed with VFXEditor" );
            ImGui.SameLine();
            ImGui.TextDisabled( "Animation canceled by movement" );
            ImGui.Unindent( 25f );
        }
    }
}
