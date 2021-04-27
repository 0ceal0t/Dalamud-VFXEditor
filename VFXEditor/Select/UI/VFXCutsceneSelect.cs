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
            base( parentId, tabId, sheet._Cutscenes, sheet._pi, dialog ) {
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
            _dialog.DisplayPath( loadedItem.Cutscene.Path );
            int vfxIdx = 0;
            foreach( var _vfx in loadedItem.VfxPaths ) {
                ImGui.Text( "VFX #" + vfxIdx + ": " );
                ImGui.SameLine();
                _dialog.DisplayPath( _vfx );
                if( ImGui.Button( "SELECT" + Id + vfxIdx ) ) {
                    _dialog.Invoke( new VFXSelectResult( VFXSelectType.GameEmote, "[CUT] " + loadedItem.Cutscene.Name + " #" + vfxIdx, _vfx ) );
                }
                ImGui.SameLine();
                _dialog.Copy( _vfx, id: Id + "Copy" + vfxIdx );
                vfxIdx++;
            }
        }

        public override string UniqueRowTitle( XivCutscene item ) {
            return item.Name + Id + item.RowId;
        }
    }
}