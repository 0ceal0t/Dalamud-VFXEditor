using ImGuiNET;
using VFXSelect.Data.Rows;

namespace VFXSelect.UI {
    public class VFXGimmickSelect : VFXSelectTab<XivGimmick, XivGimmickSelected> {
        public VFXGimmickSelect( string parentId, string tabId, VFXSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.Gimmicks, dialog ) {
        }

        public override bool CheckMatch( XivGimmick item, string searchInput ) {
            return VFXSelectDialog.Matches( item.Name, searchInput );
        }

        public override void DrawSelected( XivGimmickSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Gimmick.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( loadedItem.SelfVfxExists ) {
                ImGui.Text( "TMB Path: " );
                ImGui.SameLine();
                VFXSelectDialog.DisplayPath( loadedItem.SelfTmbPath );
                var vfxIdx = 0;
                foreach( var path in loadedItem.SelfVfxPaths ) {
                    ImGui.Text( "VFX #" + vfxIdx + ": " );
                    ImGui.SameLine();
                    VFXSelectDialog.DisplayPath( path );
                    if( ImGui.Button( "SELECT" + Id + vfxIdx ) ) {
                        Dialog.Invoke( new VFXSelectResult( VFXSelectType.GameGimmick, "[GIMMICK] " + loadedItem.Gimmick.Name + " #" + vfxIdx, path ) );
                    }
                    ImGui.SameLine();
                    VFXSelectDialog.Copy( path, id: Id + "Copy" + vfxIdx );
                    Dialog.Spawn( path, id: Id + "Spawn" + vfxIdx );
                    vfxIdx++;
                }
            }
        }

        public override string UniqueRowTitle( XivGimmick item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}
