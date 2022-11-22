using ImGuiNET;
using System;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.VfxSelect {
    public class VfxCommonSelect : VfxSelectTab<XivCommon, XivCommon> {
        private ImGuiScene.TextureWrap Icon;

        public VfxCommonSelect( string parentId, string tabId, VfxSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.Common, dialog ) {
        }

        protected override bool CheckMatch( XivCommon item, string searchInput ) => Matches( item.Name, searchInput );

        protected override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        protected override void DrawSelected( XivCommon loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            DrawIcon( Icon );

            DrawPath( "VFX Path", loadedItem.Path, Id, Dialog, SelectResultType.GameAction, "COMMON", loadedItem.Name, play: true );
        }

        protected override string UniqueRowTitle( XivCommon item ) => $"{item.Name}##{item.RowId}";
    }
}