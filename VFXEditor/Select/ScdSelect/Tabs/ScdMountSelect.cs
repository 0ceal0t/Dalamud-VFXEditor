using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.ScdSelect {
    public class ScdMountSelect : ScdSelectTab<XivMount, XivMountSelected> {
        private ImGuiScene.TextureWrap Icon;

        public ScdMountSelect( string parentId, string tabId, ScdSelectDialog dialog ) : base( parentId, tabId, SheetManager.Mounts, dialog ) { }

        protected override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        protected override bool CheckMatch( XivMount item, string searchInput ) => Matches( item.Name, searchInput );

        protected override void DrawSelected( XivMountSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Mount.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            DrawIcon( Icon );

            DrawPath( "BGM Path", loadedItem.Mount.Bgm, Id, Dialog, SelectResultType.GameNpc, "NPC", loadedItem.Mount.Name );
        }

        protected override string UniqueRowTitle( XivMount item ) => $"{item.Name}##{item.RowId}";
    }
}