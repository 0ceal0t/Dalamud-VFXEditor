using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI {
    public class VFXGimmickSelect : VFXSelectTab<XivGimmick, XivGimmickSelected> {
        public VFXGimmickSelect( string parentId, string tabId, List<XivGimmick> data, Plugin plugin, VFXSelectDialog dialog ) : base( parentId, tabId, data, plugin, dialog ) {
        }

        public override bool CheckMatch( XivGimmick item, string searchInput ) {
            return VFXSelectDialog.Matches( item.Name, searchInput );
        }

        public override void DrawSelected( XivGimmickSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Gimmick.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

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
                        _dialog.Invoke( new VFXSelectResult( VFXSelectType.GameGimmick, "[GIMMICK] " + loadedItem.Gimmick.Name + " #" + vfxIdx, _vfx ) );
                    }
                    ImGui.SameLine();
                    _dialog.Copy( _vfx, id: Id + "Copy" + vfxIdx );
                    vfxIdx++;
                }
            }
        }

        public override void Load() {
            _plugin.Manager.LoadGimmicks();
        }

        public override bool ReadyCheck() {
            return _plugin.Manager.GimmickLoaded;
        }

        public override bool SelectItem( XivGimmick item, out XivGimmickSelected loadedItem ) {
            return _plugin.Manager.SelectGimmick( item, out loadedItem );
        }

        public override string UniqueRowTitle( XivGimmick item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}
