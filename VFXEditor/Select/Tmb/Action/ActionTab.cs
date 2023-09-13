using Dalamud.Interface;
using ImGuiNET;
using System.Linq;
using VfxEditor.Utils;

namespace VfxEditor.Select.Tmb.Action {
    public class ActionTab : SelectTab<ActionRow> {
        public ActionTab( SelectDialog dialog, string name ) : this( dialog, name, "Tmb-Action" ) { }

        public ActionTab( SelectDialog dialog, string name, string stateId ) : base( dialog, name, stateId, SelectResultType.GameAction ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && ( x.IsPlayerAction || x.ClassJob.Value != null ) && !x.AffectsPosition );

            foreach( var item in sheet ) Items.Add( new ActionRow( item ) );
        }

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        protected override void DrawSelected() {
            SelectUiUtils.DrawIcon( Icon );

            DrawPath( "Start", Selected.Start.Path, $"{Selected.Name} Start", true );
            DrawMovementCancel( Selected.Start );

            DrawPath( "End", Selected.End.Path, $"{Selected.Name} End", true );
            DrawMovementCancel( Selected.End );

            DrawPath( "Hit", Selected.Hit.Path, $"{Selected.Name} Hit", true );
            DrawPath( "Weapon", Selected.Weapon.Path, $"{Selected.Name} Weapon", true );
        }

        protected override string GetName( ActionRow item ) => item.Name;

        private void DrawMovementCancel( ActionTmbData data ) {
            if( !data.IsMotionDisabled ) return;
            if( Dialog.ShowLocal ) return;
            ImGui.Indent( 25f );
            UiUtils.IconText( FontAwesomeIcon.QuestionCircle, true );
            UiUtils.Tooltip( "This parameter is set in the game's Excel sheet, and cannot be removed with VFXEditor" );
            ImGui.SameLine();
            ImGui.TextDisabled( "Animation canceled by movement" );
            ImGui.Unindent( 25f );
        }
    }
}
