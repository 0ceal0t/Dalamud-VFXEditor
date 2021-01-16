using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Plugin;
using ImGuiNET;

namespace VFXEditor.UI {
    public class VFXZoneSelect : VFXSelectTab<XivZone, XivZoneSelected> {
        public VFXZoneSelect( string parentId, string tabId, List<XivZone> data, Plugin plugin, VFXSelectDialog dialog ) : base( parentId, tabId, data, plugin, dialog ) {
        }

        public override bool CheckMatch( XivZone item, string searchInput ) {
            return VFXSelectDialog.Matches( item.Name, searchInput );
        }

        public override void DrawSelected( XivZoneSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Zone.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            ImGui.Text( "LGB Path: " );
            ImGui.SameLine();
            _dialog.DisplayPath( loadedItem.Zone.LgbPath );
            int vfxIdx = 0;
            foreach( var _vfx in loadedItem.VfxPaths ) {
                ImGui.Text( "VFX #" + vfxIdx + ": " );
                ImGui.SameLine();
                _dialog.DisplayPath( _vfx );
                if( ImGui.Button( "SELECT" + Id + vfxIdx ) ) {
                    _dialog.Invoke( new VFXSelectResult( VFXSelectType.GameZone, "[ZONE] " + loadedItem.Zone.Name + " #" + vfxIdx, _vfx ) );
                }
                ImGui.SameLine();
                _dialog.Copy( _vfx, id: Id + "Copy" + vfxIdx );
                vfxIdx++;
            }
        }

        public override void Load() {
            _plugin.Manager.LoadZones();
        }

        public override bool ReadyCheck() {
            return _plugin.Manager.ZonesLoaded;
        }

        public override bool SelectItem( XivZone item, out XivZoneSelected loadedItem ) {
            return _plugin.Manager.SelectZone( item, out loadedItem );
        }

        public override string UniqueRowTitle( XivZone item ) {
            return item.Name + Id + item.RowId;
        }
    }
}