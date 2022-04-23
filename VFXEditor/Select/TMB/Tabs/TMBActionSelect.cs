using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using Dalamud.Logging;
using ImGuiNET;
using VFXSelect.Select.Rows;

namespace VFXSelect.TMB {
    public class TMBActionSelect : TMBSelectTab<XivActionTmb, XivActionTmb> {
        private ImGuiScene.TextureWrap Icon;

        public TMBActionSelect( string parentId, string tabId, TMBSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.ActionTmb, dialog ) {
        }

        protected override bool CheckMatch( XivActionTmb item, string searchInput ) {
            return Matches( item.Name, searchInput );
        }

        protected override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        protected override void DrawSelected( XivActionTmb loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( Icon != null && Icon.ImGuiHandle != IntPtr.Zero ) {
                ImGui.Image( Icon.ImGuiHandle, new Vector2( Icon.Width, Icon.Height ) );
            }

            ImGui.Text( "Start Tmb Path: " );
            ImGui.SameLine();
            DisplayPath( loadedItem.StartTmb );
            if( !string.IsNullOrEmpty(loadedItem.StartTmb) ) {
                if( ImGui.Button( "SELECT" + Id + "Start" ) ) {
                    Dialog.Invoke( new SelectResult( SelectResultType.GameAction, "[ACTION] " + loadedItem.Name + " Start", loadedItem.StartTmb ) );
                }
                ImGui.SameLine();
                Copy( loadedItem.StartTmb, id: Id + "StartCopy" );
            }

            ImGui.Text( "End Tmb Path: " );
            ImGui.SameLine();
            DisplayPath( loadedItem.EndTmb );
            if( !string.IsNullOrEmpty( loadedItem.EndTmb ) ) {
                if( ImGui.Button( "SELECT" + Id + "End" ) ) {
                    Dialog.Invoke( new SelectResult( SelectResultType.GameAction, "[ACTION] " + loadedItem.Name + " End", loadedItem.EndTmb ) );
                }
                ImGui.SameLine();
                Copy( loadedItem.EndTmb, id: Id + "EndCopy" );
            }

            ImGui.Text( "Hit Tmb Path: " );
            ImGui.SameLine();
            DisplayPath( loadedItem.HitTmb );
            if( !string.IsNullOrEmpty( loadedItem.HitTmb ) ) {
                if( ImGui.Button( "SELECT" + Id + "Hit" ) ) {
                    Dialog.Invoke( new SelectResult( SelectResultType.GameAction, "[ACTION] " + loadedItem.Name + " Hit", loadedItem.HitTmb ) );
                }
                ImGui.SameLine();
                Copy( loadedItem.HitTmb, id: Id + "HitCopy" );
            }

            ImGui.Text( "Weapon Tmb Path: " );
            ImGui.SameLine();
            DisplayPath( loadedItem.WeaponTmb );
            if( !string.IsNullOrEmpty( loadedItem.WeaponTmb ) ) {
                if( ImGui.Button( "SELECT" + Id + "Weapon" ) ) {
                    Dialog.Invoke( new SelectResult( SelectResultType.GameAction, "[WEAPON] " + loadedItem.Name + " Weapon", loadedItem.WeaponTmb ) );
                }
                ImGui.SameLine();
                Copy( loadedItem.WeaponTmb, id: Id + "WeaponCopy" );
            }
        }

        protected override string UniqueRowTitle( XivActionTmb item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}
