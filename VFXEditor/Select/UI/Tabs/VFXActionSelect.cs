using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using VFXSelect.Data.Rows;

namespace VFXSelect.UI {
    public class VFXActionSelect : VFXSelectTab<XivActionBase, XivActionSelected> {
        private ImGuiScene.TextureWrap Icon;

        public VFXActionSelect( string parentId, string tabId, VFXSelectDialog dialog, bool nonPlayer = false ) :
            base( parentId, tabId, !nonPlayer ? SheetManager.Actions : SheetManager.NonPlayerActions, dialog ) {
        }

        public override bool CheckMatch( XivActionBase item, string searchInput ) {
            return VFXSelectDialog.Matches( item.Name, searchInput );
        }

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
            VFXSelectDialog.DisplayPath( loadedItem.CastVfxPath );
            if( loadedItem.CastVfxExists ) {
                if( ImGui.Button( "SELECT" + Id + "Cast" ) ) {
                    Dialog.Invoke( new VFXSelectResult( VFXSelectType.GameAction, "[ACTION] " + loadedItem.Action.Name + " Cast", loadedItem.CastVfxPath ) );
                }
                ImGui.SameLine();
                VFXSelectDialog.Copy( loadedItem.CastVfxPath, id: Id + "CastCopy" );
                Dialog.Spawn( loadedItem.CastVfxPath, id: Id + "CastSpawn" );
            }

            if( loadedItem.SelfVfxExists ) {
                ImGui.Text( "TMB Path: " );
                ImGui.SameLine();
                VFXSelectDialog.DisplayPath( loadedItem.SelfTmbPath );
                var vfxIdx = 0;
                foreach( var path in loadedItem.SelfVfxPaths ) {
                    ImGui.Text( "VFX #" + vfxIdx + ": " );
                    ImGui.SameLine();
                    VFXSelectDialog.DisplayPath( path );
                    if( ImGui.Button( "SELECT" + Id + vfxIdx ) ) {
                        Dialog.Invoke( new VFXSelectResult( VFXSelectType.GameAction, "[ACTION] " + loadedItem.Action.Name + " #" + vfxIdx, path ) );
                    }
                    ImGui.SameLine();
                    VFXSelectDialog.Copy( path, id: Id + "Copy" + vfxIdx );
                    Dialog.Spawn( path, id: Id + "Spawn" + vfxIdx );
                    vfxIdx++;
                }
            }
        }

        public override string UniqueRowTitle( XivActionBase item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}
