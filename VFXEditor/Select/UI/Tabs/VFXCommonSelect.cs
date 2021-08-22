using System.Numerics;
using ImGuiNET;
using VFXSelect.Data.Rows;

namespace VFXSelect.UI {
    public class VFXCommonSelect : VFXSelectTab<XivCommon, XivCommon> {
        private ImGuiScene.TextureWrap Icon;

        public VFXCommonSelect( string parentId, string tabId, VFXSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.Misc, dialog ) {
        }

        public override bool CheckMatch( XivCommon item, string searchInput ) {
            return VFXSelectDialog.Matches( item.Name, searchInput );
        }

        public override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        public override void DrawSelected( XivCommon loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( Icon != null ) {
                ImGui.Image( Icon.ImGuiHandle, new Vector2( Icon.Width, Icon.Height ) );
            }

            ImGui.Text( "VFX Path: " );
            ImGui.SameLine();
            VFXSelectDialog.DisplayPath( loadedItem.VfxPath );

            if( ImGui.Button( "SELECT" + Id ) ) {
                Dialog.Invoke( new VFXSelectResult( VFXSelectType.GameStatus, "[COMMON] " + loadedItem.Name, loadedItem.VfxPath ) );
            }
            ImGui.SameLine();
            VFXSelectDialog.Copy( loadedItem.VfxPath, id: Id + "Copy" );
            Dialog.Spawn( loadedItem.VfxPath, id: Id + "Spawn" );
        }

        public override string UniqueRowTitle( XivCommon item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}