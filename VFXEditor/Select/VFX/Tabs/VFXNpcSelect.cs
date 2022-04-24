using System.Diagnostics;
using ImGuiNET;
using VFXSelect.Select.Rows;

namespace VFXSelect.VFX {
    public class VFXNpcSelect : VFXSelectTab<XivNpc, XivNpcSelected> {
        public VFXNpcSelect( string parentId, string tabId, VFXSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.Npcs, dialog ) {
        }

        protected override bool CheckMatch( XivNpc item, string searchInput ) {
            return Matches( item.Name, searchInput ) || Matches( item.Id, searchInput );
        }

        protected override void DrawExtra() {
            ImGui.Text( "Big thanks to Ani and the rest of the Anamnesis/CMTools team " );
            ImGui.SameLine();
            if( ImGui.SmallButton( "Github##Anamnesis" ) ) {
                Process.Start( new ProcessStartInfo {
                    FileName = "https://github.com/imchillin/Anamnesis",
                    UseShellExecute = true
                } );
            }

        }

        protected override void DrawSelected( XivNpcSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Npc.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( "Variant: " + loadedItem.Npc.Variant );

            DrawPath( "VFX", loadedItem.VfxPaths, Id, Dialog, SelectResultType.GameZone, "NPC", loadedItem.Npc.Name, spawn: true );
        }

        protected override string UniqueRowTitle( XivNpc item ) {
            return item.Name + Id + item.RowId;
        }
    }
}