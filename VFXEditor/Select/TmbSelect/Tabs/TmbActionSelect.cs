using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.TmbSelect {
    public class TmbActionSelect : SelectTab<XivActionTmb> {
        private ImGuiScene.TextureWrap Icon;

        public TmbActionSelect( string tabId, TmbSelectDialog dialog, bool nonPlayer = false ) : base( tabId, nonPlayer ? SheetManager.NonPlayerActionTmb : SheetManager.ActionTmb, dialog ) { }

        protected override void OnSelect() => LoadIcon( Selected.Icon, ref Icon );

        protected override void DrawSelected( string parentId ) {
            DrawIcon( Icon );

            DrawPath( "Start Tmb Path", Selected.StartTmb, $"{parentId}/Start", SelectResultType.GameAction, $"{Selected.Name} Start", true );
            DrawPath( "End Tmb Path", Selected.EndTmb, $"{parentId}/End", SelectResultType.GameAction, $"{Selected.Name} End", true );
            DrawPath( "Hit Tmb Path", Selected.HitTmb, $"{parentId}/Hit", SelectResultType.GameAction, $"{Selected.Name} Hit", true );
            DrawPath( "Weapon Tmb Path", Selected.WeaponTmb, $"{parentId}/Weapon", SelectResultType.GameAction, $"{Selected.Name} Weapon", true );
        }

        protected override string GetName( XivActionTmb item ) => item.Name;
    }
}
