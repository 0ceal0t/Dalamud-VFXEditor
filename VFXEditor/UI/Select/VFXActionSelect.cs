using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI
{
    public class VFXActionSelect : VFXSelectTab<XivActionBase, XivActionSelected> {
        public VFXActionSelect( string parentId, string tabId, List<XivActionBase> data, Plugin plugin, VFXSelectDialog dialog ) : base( parentId, tabId, data, plugin, dialog ) {
        }

        public override bool CheckMatch( XivActionBase item, string searchInput ) {
            return VFXSelectDialog.Matches( item.Name, searchInput );
        }

        public override void DrawSelected( XivActionSelected loadedItem ) {
            ImGui.Text( loadedItem.Action.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            ImGui.Text( "Cast VFX Path: " );
            ImGui.SameLine();
            _dialog.DisplayPath( loadedItem.CastVfxPath );
            if( loadedItem.CastVfxExists ) {
                if( ImGui.Button( "SELECT" + Id + "Cast" ) ) {
                    _dialog.Invoke( new VFXSelectResult( VFXSelectType.GameAction, "[ACTION] " + loadedItem.Action.Name, loadedItem.CastVfxPath ) );
                }
                ImGui.SameLine();
                _dialog.Copy( loadedItem.CastVfxPath, id: Id + "CastCopy" );
            }

            if( loadedItem.SelfVfxExists ) {
                ImGui.Text( "TMB Path: " );
                ImGui.SameLine();
                _dialog.DisplayPath( loadedItem.SelfTmbPath );
                int vfxIdx = 0;
                foreach( var _vfx in loadedItem.SelfVfxPaths ) {
                    ImGui.Text( "VFX #" + vfxIdx + ": " );
                    ImGui.SameLine();
                    _dialog.DisplayPath( _vfx );
                    if( ImGui.Button( "SELECT" + Id + vfxIdx ) ) {
                        _dialog.Invoke( new VFXSelectResult( VFXSelectType.GameAction, "[ACTION] " + loadedItem.Action.Name, _vfx ) );
                    }
                    ImGui.SameLine();
                    _dialog.Copy( _vfx, id: Id + "Copy" + vfxIdx );
                    vfxIdx++;
                }
            }
        }

        public override bool SelectItem( XivActionBase item, out XivActionSelected loadedItem ) {
            return _plugin.Manager.SelectAction( item, out loadedItem );
        }

        public override string UniqueRowTitle( XivActionBase item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}
