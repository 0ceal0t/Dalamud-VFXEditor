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
    public class VFXNpcSelect : VFXSelectTab<XivNpc, XivNpcSelected> {
        public VFXNpcSelect( string parentId, string tabId, SheetManager sheet, VFXSelectDialog dialog ) : 
            base( parentId, tabId, sheet._Npcs, sheet._pi, dialog ) {
        }

        public override bool CheckMatch( XivNpc item, string searchInput ) {
            return VFXSelectDialog.Matches( item.Name, searchInput );
        }

        public override void DrawExtra() {
            ImGui.Text( "Big thanks to Ani and the rest of the Anamnesis/CMTools team " );
            ImGui.SameLine();
            if( ImGui.SmallButton( "Github##Anamnesis" ) ) {
                Process.Start( "https://github.com/imchillin/Anamnesis" );
            }

        }

        public override void DrawSelected( XivNpcSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Npc.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( "Variant: " + loadedItem.Npc.Variant );
            ImGui.Text( "IMC Count: " + loadedItem.Count );

            ImGui.Text( "IMC Path: " );
            ImGui.SameLine();
            _dialog.DisplayPath( loadedItem.ImcPath );

            int vfxIdx = 0;
            foreach( var _vfx in loadedItem.VfxPaths ) {
                ImGui.Text( "VFX #" + vfxIdx + ": " );
                ImGui.SameLine();
                _dialog.DisplayPath( _vfx );
                if( ImGui.Button( "SELECT" + Id + vfxIdx ) ) {
                    _dialog.Invoke( new VFXSelectResult( VFXSelectType.GameNpc, "[NPC] " + loadedItem.Npc.Name + " #" + vfxIdx, _vfx ) );
                }
                ImGui.SameLine();
                _dialog.Copy( _vfx, id: Id + "Copy" + vfxIdx );
                vfxIdx++;
            }
        }

        public override string UniqueRowTitle( XivNpc item ) {
            return item.Name + Id + item.RowId;
        }
    }
}