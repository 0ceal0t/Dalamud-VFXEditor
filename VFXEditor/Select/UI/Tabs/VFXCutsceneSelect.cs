using ImGuiNET;
using VFXSelect.Data.Rows;

namespace VFXSelect.UI {
    public class VFXCutsceneSelect : VFXSelectTab<XivCutscene, XivCutsceneSelected> {
        public VFXCutsceneSelect( string parentId, string tabId, VFXSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.Cutscenes, dialog ) {
        }

        public override bool CheckMatch( XivCutscene item, string searchInput ) {
            return VFXSelectDialog.Matches( item.Name, searchInput );
        }

        public override void DrawSelected( XivCutsceneSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Cutscene.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            ImGui.Text( "CUTB Path: " );
            ImGui.SameLine();
            VFXSelectDialog.DisplayPath( loadedItem.Cutscene.Path );
            var vfxIdx = 0;
            foreach( var path in loadedItem.VfxPaths ) {
                ImGui.Text( "VFX #" + vfxIdx + ": " );
                ImGui.SameLine();
                VFXSelectDialog.DisplayPath( path );
                if( ImGui.Button( "SELECT" + Id + vfxIdx ) ) {
                    Dialog.Invoke( new VFXSelectResult( VFXSelectType.GameEmote, "[CUT] " + loadedItem.Cutscene.Name + " #" + vfxIdx, path ) );
                }
                ImGui.SameLine();
                VFXSelectDialog.Copy( path, id: Id + "Copy" + vfxIdx );
                Dialog.Spawn( path, id: Id + "Spawn" + vfxIdx );
                vfxIdx++;
            }
        }

        public override string UniqueRowTitle( XivCutscene item ) {
            return item.Name + Id + item.RowId;
        }
    }
}