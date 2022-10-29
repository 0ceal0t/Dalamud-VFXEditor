using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.VfxSelect {
    public class VfxGimmickSelect : VfxSelectTab<XivGimmick, XivGimmickSelected> {
        public VfxGimmickSelect( string parentId, string tabId, VfxSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.Gimmicks, dialog ) {
        }

        protected override bool CheckMatch( XivGimmick item, string searchInput ) {
            return Matches( item.Name, searchInput );
        }

        protected override void DrawSelected( XivGimmickSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Gimmick.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( loadedItem.VfxExists ) {
                ImGui.Text( "TMB Path: " );
                ImGui.SameLine();
                DisplayPath( loadedItem.TmbPath );
                Copy( loadedItem.TmbPath, id: Id + "CopyTmb" );

                DrawPath( "VFX", loadedItem.VfxPaths, Id, Dialog, SelectResultType.GameGimmick, "GIMMICK", loadedItem.Gimmick.Name, spawn: true );
            }
        }

        protected override string UniqueRowTitle( XivGimmick item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}
