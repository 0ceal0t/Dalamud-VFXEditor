using ImGuiNET;
using VFXEditor.Select.Rows;

namespace VFXEditor.Select.TMB {
    public class TMBCommonSelect : TMBSelectTab<XivCommon, XivCommon> {
        private ImGuiScene.TextureWrap Icon;

        public TMBCommonSelect( string parentId, string tabId, TMBSelectDialog dialog ) :
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

            DrawPath( "TMB Path", loadedItem.Path, Id, Dialog, SelectResultType.GameAction, "COMMON", loadedItem.Name, spawn: true );
        }

        protected override string UniqueRowTitle( XivCommon item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}