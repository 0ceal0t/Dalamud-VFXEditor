using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.VfxSelect {
    public class VfxItemSelect : VfxSelectTab<XivItem, XivItemSelected> {
        private ImGuiScene.TextureWrap Icon;

        public VfxItemSelect( string parentId, string tabId, VfxSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.Items, dialog ) {
        }

        protected override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        protected override bool CheckMatch( XivItem item, string searchInput ) {
            return Matches( item.Name, searchInput );
        }

        protected override void DrawSelected( XivItemSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Item.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            DrawIcon( Icon );

            ImGui.Text( "Variant: " + loadedItem.Item.Variant );

            ImGui.Text( "IMC Path: " );
            ImGui.SameLine();
            DisplayPath( loadedItem.ImcPath );

            DrawPath( "VFX Path", loadedItem.GetVfxPath(), Id, Dialog, SelectResultType.GameItem, "ITEM", loadedItem.Item.Name, spawn: true );
        }

        protected override string UniqueRowTitle( XivItem item ) {
            return item.Name + Id;
        }
    }
}