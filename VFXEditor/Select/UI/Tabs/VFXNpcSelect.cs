using System.Diagnostics;
using ImGuiNET;
using VFXSelect.Data.Rows;

namespace VFXSelect.UI {
    public class VFXNpcSelect : VFXSelectTab<XivNpc, XivNpcSelected> {
        public VFXNpcSelect( string parentId, string tabId, VFXSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.Npcs, dialog ) {
        }

        public override bool CheckMatch( XivNpc item, string searchInput ) {
            return VFXSelectDialog.Matches( item.Name, searchInput ) || VFXSelectDialog.Matches( item.Id, searchInput );
        }

        public override void DrawExtra() {
            ImGui.Text( "Big thanks to Ani and the rest of the Anamnesis/CMTools team " );
            ImGui.SameLine();
            if( ImGui.SmallButton( "Github##Anamnesis" ) ) {
                Process.Start( new ProcessStartInfo {
                    FileName = "https://github.com/imchillin/Anamnesis",
                    UseShellExecute = true
                } );
            }

        }

        public override void DrawSelected( XivNpcSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Npc.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( "Variant: " + loadedItem.Npc.Variant );

            var vfxIdx = 0;
            foreach( var path in loadedItem.VfxPaths ) {
                ImGui.Text( "VFX #" + vfxIdx + ": " );
                ImGui.SameLine();
                VFXSelectDialog.DisplayPath( path );
                if( ImGui.Button( "SELECT" + Id + vfxIdx ) ) {
                    Dialog.Invoke( new VFXSelectResult( VFXSelectType.GameNpc, "[NPC] " + loadedItem.Npc.Name + " #" + vfxIdx, path ) );
                }
                ImGui.SameLine();
                VFXSelectDialog.Copy( path, id: Id + "Copy" + vfxIdx );
                Dialog.Spawn( path, id: Id + "Spawn" + vfxIdx );
                vfxIdx++;
            }
        }

        public override string UniqueRowTitle( XivNpc item ) {
            return item.Name + Id + item.RowId;
        }
    }
}