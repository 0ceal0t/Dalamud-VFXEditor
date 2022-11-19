using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.TmbSelect {
    public class TmbCommonSelect : TmbSelectTab<XivCommon, XivCommon> {
        private ImGuiScene.TextureWrap Icon;

        public TmbCommonSelect( string parentId, string tabId, TmbSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.MiscTmb, dialog ) {
        }

        protected override bool CheckMatch( XivCommon item, string searchInput ) {
            return Matches( item.Name, searchInput );
        }

        protected override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        protected override void DrawSelected( XivCommon loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            DrawIcon( Icon );

            DrawPath( "TMB Path", loadedItem.Path, Id, Dialog, SelectResultType.GameAction, "COMMON", loadedItem.Name, true );
        }

        protected override string UniqueRowTitle( XivCommon item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}