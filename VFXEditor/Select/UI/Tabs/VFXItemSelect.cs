using System.Numerics;
using ImGuiNET;
using VFXSelect.Data.Rows;

namespace VFXSelect.UI {
    public class VFXItemSelect : VFXSelectTab<XivItem, XivItemSelected> {
        private ImGuiScene.TextureWrap Icon;

        public VFXItemSelect( string parentId, string tabId, VFXSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.Items, dialog ) {
        }

        public override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        public override bool CheckMatch( XivItem item, string searchInput ) {
            return VFXSelectDialog.Matches( item.Name, searchInput );
        }

        public override void DrawSelected( XivItemSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Item.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( Icon != null ) {
                ImGui.Image( Icon.ImGuiHandle, new Vector2( Icon.Width, Icon.Height ) );
            }

            ImGui.Text( "Variant: " + loadedItem.Item.Variant );
            ImGui.Text( "IMC Count: " + loadedItem.Count );
            ImGui.Text( "VFX Id: " + loadedItem.VfxId );

            ImGui.Text( "IMC Path: " );
            ImGui.SameLine();
            VFXSelectDialog.DisplayPath( loadedItem.ImcPath );

            ImGui.Text( "VFX Path: " );
            ImGui.SameLine();
            VFXSelectDialog.DisplayPath( loadedItem.GetVFXPath() );
            if( loadedItem.VfxExists ) {
                if( ImGui.Button( "SELECT" + Id ) ) {
                    Dialog.Invoke( new VFXSelectResult( VFXSelectType.GameItem, "[ITEM] " + loadedItem.Item.Name, loadedItem.GetVFXPath() ) );
                }
                ImGui.SameLine();
                VFXSelectDialog.Copy( loadedItem.GetVFXPath(), id: Id + "Copy" );
                Dialog.Spawn( loadedItem.GetVFXPath(), id: Id + "Spawn" );
            }
        }

        public override string UniqueRowTitle( XivItem item ) {
            return item.Name + Id;
        }
    }
}