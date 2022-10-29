using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.TmbSelect {
    public class TmbActionSelect : TmbSelectTab<XivActionTmb, XivActionTmb> {
        private ImGuiScene.TextureWrap Icon;

        public TmbActionSelect( string parentId, string tabId, TmbSelectDialog dialog, bool nonPlayer = false ) :
            base( parentId, tabId, nonPlayer ? SheetManager.NonPlayerActionTmb : SheetManager.ActionTmb, dialog ) {
        }

        protected override bool CheckMatch( XivActionTmb item, string searchInput ) {
            return Matches( item.Name, searchInput );
        }

        protected override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        protected override void DrawSelected( XivActionTmb loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            DrawIcon( Icon );

            DrawPath( "Start Tmb Path", loadedItem.StartTmb, Id + "Start", Dialog, SelectResultType.GameAction, "ACTION", loadedItem.Name + " Start" );

            DrawPath( "End Tmb Path", loadedItem.EndTmb, Id + "End", Dialog, SelectResultType.GameAction, "ACTION", loadedItem.Name + " End" );

            DrawPath( "Hit Tmb Path", loadedItem.HitTmb, Id + "Hit", Dialog, SelectResultType.GameAction, "ACTION", loadedItem.Name + " Hit" );

            DrawPath( "Weapon Tmb Path", loadedItem.WeaponTmb, Id + "Weapon", Dialog, SelectResultType.GameAction, "ACTION", loadedItem.Name + " Weapon" );
        }

        protected override string UniqueRowTitle( XivActionTmb item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}
