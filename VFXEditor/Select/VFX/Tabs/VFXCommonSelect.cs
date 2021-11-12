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

            if( Icon != null ) {
                ImGui.Image( Icon.ImGuiHandle, new Vector2( Icon.Width, Icon.Height ) );
            }

            ImGui.Text( "VFX Path: " );
            ImGui.SameLine();
            DisplayPath( loadedItem.VfxPath );

            if( ImGui.Button( "SELECT" + Id ) ) {
                Dialog.Invoke( new SelectResult( SelectResultType.GameStatus, "[COMMON] " + loadedItem.Name, loadedItem.VfxPath ) );
            }
            ImGui.SameLine();
            Copy( loadedItem.VfxPath, id: Id + "Copy" );
            Dialog.Spawn( loadedItem.VfxPath, id: Id + "Spawn" );
        }

        protected override string UniqueRowTitle( XivCommon item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}