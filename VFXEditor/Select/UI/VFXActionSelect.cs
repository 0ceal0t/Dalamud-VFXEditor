using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ImGuiNET;
using VFXSelect.Data.Rows;

namespace VFXSelect.UI
{
    public class VFXActionSelect : VFXSelectTab<XivActionBase, XivActionSelected> {
        public VFXActionSelect( string parentId, string tabId, SheetManager sheet, VFXSelectDialog dialog, bool nonPlayer = false ) :
            base( parentId, tabId, !nonPlayer ? sheet._Actions : sheet._NonPlayerActions, sheet._pi, dialog ) {
        }

        public override bool CheckMatch( XivActionBase item, string searchInput ) {
            return VFXSelectDialog.Matches( item.Name, searchInput );
        }

        ImGuiScene.TextureWrap Icon;
        public override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        public override void DrawSelected( XivActionSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Action.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( Icon != null ) {
                ImGui.Image( Icon.ImGuiHandle, new Vector2( Icon.Width, Icon.Height ) );
            }

            ImGui.Text( "Cast VFX Path: " );
            ImGui.SameLine();
            _dialog.DisplayPath( loadedItem.CastVfxPath );
            if( loadedItem.CastVfxExists ) {
                if( ImGui.Button( "SELECT" + Id + "Cast" ) ) {
                    _dialog.Invoke( new VFXSelectResult( VFXSelectType.GameAction, "[ACTION] " + loadedItem.Action.Name + " Cast", loadedItem.CastVfxPath ) );
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
                        _dialog.Invoke( new VFXSelectResult( VFXSelectType.GameAction, "[ACTION] " + loadedItem.Action.Name + " #" + vfxIdx, _vfx ) );
                    }
                    ImGui.SameLine();
                    _dialog.Copy( _vfx, id: Id + "Copy" + vfxIdx );
                    vfxIdx++;
                }
            }
        }

        public override string UniqueRowTitle( XivActionBase item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}
