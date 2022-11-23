using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.TmbSelect {
    public class TmbActionSelect : SelectTab<XivActionTmb, XivActionTmb> {
        private ImGuiScene.TextureWrap Icon;

        public TmbActionSelect( string tabId, TmbSelectDialog dialog, bool nonPlayer = false ) : base( tabId, nonPlayer ? SheetManager.NonPlayerActionTmb : SheetManager.ActionTmb, dialog ) { }

        protected override void OnSelect() => LoadIcon( Selected.Icon, ref Icon );

        protected override void DrawSelected( string parentId ) {
            DrawIcon( Icon );

            DrawPath( "Start Tmb Path", Loaded.StartTmb, $"{parentId}/Start", SelectResultType.GameAction, $"{Loaded.Name} Start", true );
            DrawPath( "End Tmb Path", Loaded.EndTmb, $"{parentId}/End", SelectResultType.GameAction, $"{Loaded.Name} End", true );
            DrawPath( "Hit Tmb Path", Loaded.HitTmb, $"{parentId}/Hit", SelectResultType.GameAction, $"{Loaded.Name} Hit", true );
            DrawPath( "Weapon Tmb Path", Loaded.WeaponTmb, $"{parentId}/Weapon", SelectResultType.GameAction, $"{Loaded.Name} Weapon", true );
        }

        protected override string GetName( XivActionTmb item ) => item.Name;
    }
}
