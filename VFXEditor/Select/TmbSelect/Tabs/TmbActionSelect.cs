using Dalamud.Interface;
using ImGuiNET;
using VfxEditor.Select.Rows;
using VfxEditor.Utils;
using static VfxEditor.Select.Rows.XivActionTmb;

namespace VfxEditor.Select.TmbSelect {
    public class TmbActionSelect : SelectTab<XivActionTmb> {
        private ImGuiScene.TextureWrap Icon;

        public TmbActionSelect( string tabId, TmbSelectDialog dialog, bool nonPlayer = false ) : base( tabId, nonPlayer ? SheetManager.NonPlayerActionTmb : SheetManager.ActionTmb, dialog ) { }

        protected override void OnSelect() => LoadIcon( Selected.Icon, ref Icon );

        protected override void DrawSelected( string parentId ) {
            DrawIcon( Icon );

            DrawPath( "Start", Selected.Start.Path, $"{parentId}/Start", SelectResultType.GameAction, $"{Selected.Name} Start", true );
            DrawMovementCancel( Selected.Start );

            DrawPath( "End", Selected.End.Path, $"{parentId}/End", SelectResultType.GameAction, $"{Selected.Name} End", true );
            DrawMovementCancel( Selected.End );

            DrawPath( "Hit", Selected.Hit.Path, $"{parentId}/Hit", SelectResultType.GameAction, $"{Selected.Name} Hit", true );
            DrawPath( "Weapon", Selected.Weapon.Path, $"{parentId}/Weapon", SelectResultType.GameAction, $"{Selected.Name} Weapon", true );
        }

        protected override string GetName( XivActionTmb item ) => item.Name;

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
