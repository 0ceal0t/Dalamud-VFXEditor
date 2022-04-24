using System.Numerics;
using ImGuiNET;
using VFXSelect.Select.Rows;

namespace VFXSelect.VFX {
    public class VFXCommonSelect : VFXSelectTab<XivCommon, XivCommon> {
        private ImGuiScene.TextureWrap Icon;

        public VFXCommonSelect( string parentId, string tabId, VFXSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.Misc, dialog ) {
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

            DrawPath( "VFX Path", loadedItem.VfxPath, Id, Dialog, SelectResultType.GameAction, "COMMON", loadedItem.Name, spawn: true );
        }

        protected override string UniqueRowTitle( XivCommon item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}