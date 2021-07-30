using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Plugin;
using ImGuiNET;
using VFXSelect.Data.Rows;

namespace VFXSelect.UI {
    public class VFXCutsceneSelect : VFXSelectTab<XivCutscene, XivCutsceneSelected> {
        public VFXCutsceneSelect( string parentId, string tabId, SheetManager sheet, VFXSelectDialog dialog ) : 
            base( parentId, tabId, sheet.Cutscenes, sheet.PluginInterface, dialog ) {
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