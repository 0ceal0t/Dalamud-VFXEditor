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
    public class VFXStatusSelect : VFXSelectTab<XivStatus, XivStatus> {
        public VFXStatusSelect( string parentId, string tabId, SheetManager sheet, VFXSelectDialog dialog ) : 
            base( parentId, tabId, sheet._Statuses, sheet._pi, dialog ) {
        }

        public override bool CheckMatch( XivStatus item, string searchInput ) {
            return VFXSelectDialog.Matches( item.Name, searchInput );
        }

        ImGuiScene.TextureWrap Icon;
        public override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        public override void DrawSelected( XivStatus loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if(Icon != null ) {
                ImGui.Image( Icon.ImGuiHandle, new Vector2( Icon.Width, Icon.Height ) );
            }

            // ==== LOOP 1 =====
            ImGui.Text( "Loop VFX1: " );
            ImGui.SameLine();
            _dialog.DisplayPath( loadedItem.GetLoopVFX1Path() );
            if( loadedItem.LoopVFX1Exists ) {
                if( ImGui.Button( "SELECT" + Id + "Loop1" ) ) {
                    _dialog.Invoke( new VFXSelectResult( VFXSelectType.GameStatus, "[STATUS] " + loadedItem.Name + " Loop1", loadedItem.GetLoopVFX1Path() ) );
                }
                ImGui.SameLine();
                _dialog.Copy( loadedItem.GetLoopVFX1Path(), id: Id + "Loop1Copy" );
            }
            // ==== LOOP 2 =====
            ImGui.Text( "Loop VFX2: " );
            ImGui.SameLine();
            _dialog.DisplayPath( loadedItem.GetLoopVFX2Path() );
            if( loadedItem.LoopVFX2Exists ) {
                if( ImGui.Button( "SELECT" + Id + "Loop2" ) ) {
                    _dialog.Invoke( new VFXSelectResult( VFXSelectType.GameStatus, "[STATUS] " + loadedItem.Name + " Loop2", loadedItem.GetLoopVFX2Path() ) );
                }
                ImGui.SameLine();
                _dialog.Copy( loadedItem.GetLoopVFX2Path(), id: Id + "Loop2Copy" );
            }
            // ==== LOOP 3 =====
            ImGui.Text( "Loop VFX3: " );
            ImGui.SameLine();
            _dialog.DisplayPath( loadedItem.GetLoopVFX3Path() );
            if( loadedItem.LoopVFX3Exists ) {
                if( ImGui.Button( "SELECT" + Id + "Loop3" ) ) {
                    _dialog.Invoke( new VFXSelectResult( VFXSelectType.GameStatus, "[STATUS] " + loadedItem.Name + " Loop3", loadedItem.GetLoopVFX3Path() ) );
                }
                ImGui.SameLine();
                _dialog.Copy( loadedItem.GetLoopVFX3Path(), id: Id + "Loop3Copy" );
            }
        }

        public override string UniqueRowTitle( XivStatus item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}