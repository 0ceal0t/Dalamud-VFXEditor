using ImGuiNET;
using VFXEditor.Select.Rows;

namespace VFXEditor.Select.VFX {
    public class VFXStatusSelect : VFXSelectTab<XivStatus, XivStatus> {
        private ImGuiScene.TextureWrap Icon;

        public VFXStatusSelect( string parentId, string tabId, VFXSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.Statuses, dialog ) {
        }

        protected override bool CheckMatch( XivStatus item, string searchInput ) {
            return Matches( item.Name, searchInput );
        }

        protected override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        protected override void DrawSelected( XivStatus loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            DrawIcon( Icon );

            DrawPath( "Hit VFX", loadedItem.HitVFXPath, Id + "Hit", Dialog, SelectResultType.GameStatus, "STATUS", loadedItem.Name + " Hit", spawn: true );
            DrawPath( "Loop VFX 1", loadedItem.LoopVFXPath1, Id + "Loop1", Dialog, SelectResultType.GameStatus, "STATUS", loadedItem.Name + " Loop 1", spawn: true );
            DrawPath( "Loop VFX 2", loadedItem.LoopVFXPath2, Id + "Loop2", Dialog, SelectResultType.GameStatus, "STATUS", loadedItem.Name + " Loop 2", spawn: true );
            DrawPath( "Loop VFX 3", loadedItem.LoopVFXPath3, Id + "Loop3", Dialog, SelectResultType.GameStatus, "STATUS", loadedItem.Name + " Loop 3", spawn: true );
        }

        protected override string UniqueRowTitle( XivStatus item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}