using ImGuiNET;
using VFXEditor.Select.Rows;

namespace VFXEditor.Select.VFX {
    public class VFXHousingSelect : VFXSelectTab<XivHousing, XivHousingSelected> {
        private ImGuiScene.TextureWrap Icon;

        public VFXHousingSelect( string parentId, string tabId, VFXSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.Housing, dialog ) {
        }

        protected override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        protected override bool CheckMatch( XivHousing item, string searchInput ) {
            return Matches( item.Name, searchInput );
        }

        protected override void DrawSelected( XivHousingSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Housing.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            DrawIcon( Icon );

            ImGui.Text( "SGB Path: " );
            ImGui.SameLine();
            DisplayPath( loadedItem.Housing.sgbPath );

            DrawPath( "VFX", loadedItem.VfxPaths, Id, Dialog, SelectResultType.GameItem, "HOUSING", loadedItem.Housing.Name, spawn: true );
        }

        protected override string UniqueRowTitle( XivHousing item ) {
            return item.Name + Id;
        }
    }
}