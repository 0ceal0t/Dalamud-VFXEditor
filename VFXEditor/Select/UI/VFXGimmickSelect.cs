using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ImGuiNET;
using VFXSelect.Data.Rows;

namespace VFXSelect.UI {
    public class VFXGimmickSelect : VFXSelectTab<XivGimmick, XivGimmickSelected> {
        public VFXGimmickSelect( string parentId, string tabId, SheetManager sheet, VFXSelectDialog dialog ) : 
            base( parentId, tabId, sheet._Gimmicks, sheet._pi, dialog ) {
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

        public override string UniqueRowTitle( XivGimmick item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}
