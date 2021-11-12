using ImGuiNET;
using VFXSelect.Select.Rows;

namespace VFXSelect.VFX {
    public class VFXGimmickSelect : VFXSelectTab<XivGimmick, XivGimmickSelected> {
        public VFXGimmickSelect( string parentId, string tabId, VFXSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.Gimmicks, dialog ) {
        }

        protected override bool CheckMatch( XivGimmick item, string searchInput ) {
            return Matches( item.Name, searchInput );
        }

        protected override void DrawSelected( XivGimmickSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Gimmick.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( loadedItem.SelfVfxExists ) {
                ImGui.Text( "TMB Path: " );
                ImGui.SameLine();
                DisplayPath( loadedItem.SelfTmbPath );
                Copy( loadedItem.SelfTmbPath, id: Id + "CopyTmb" );

                var vfxIdx = 0;
                foreach( var path in loadedItem.SelfVfxPaths ) {
                    ImGui.Text( "VFX #" + vfxIdx + ": " );
                    ImGui.SameLine();
                    DisplayPath( path );
                    if( ImGui.Button( "SELECT" + Id + vfxIdx ) ) {
                        Dialog.Invoke( new SelectResult( SelectResultType.GameGimmick, "[GIMMICK] " + loadedItem.Gimmick.Name + " #" + vfxIdx, path ) );
                    }
                    ImGui.SameLine();
                    Copy( path, id: Id + "Copy" + vfxIdx );
                    Dialog.Spawn( path, id: Id + "Spawn" + vfxIdx );
                    vfxIdx++;
                }
            }
        }

        protected override string UniqueRowTitle( XivGimmick item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}
