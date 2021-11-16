using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using Dalamud.Logging;
using ImGuiNET;
using VFXSelect.Select.Rows;

namespace VFXSelect.VFX {
    public class PapActionSelect : PapSelectTab<XivPap, XivPap> {
        private ImGuiScene.TextureWrap Icon;

        public PapActionSelect( string parentId, string tabId, PapSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.PapSheetLoader, dialog ) {
        }

        protected override bool CheckMatch( XivPap item, string searchInput ) {
            return Matches( item.Name, searchInput );
        }

        protected override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        protected override void DrawSelected( XivPap loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( Icon != null && Icon.ImGuiHandle != IntPtr.Zero ) {
                ImGui.Image( Icon.ImGuiHandle, new Vector2( Icon.Width, Icon.Height ) );
            }

            ImGui.Text( "Start Pap Path: " );
            ImGui.SameLine();
            DisplayPath( loadedItem.StartPap );
            if( !string.IsNullOrEmpty(loadedItem.StartPap ) ) {
                if( ImGui.Button( "SELECT" + Id + "Start" ) ) {
                    Dialog.Invoke( new SelectResult( SelectResultType.GameAction, "[ACTION] " + loadedItem.Name + " Start", loadedItem.StartPap ) );
                }
                ImGui.SameLine();
                Copy( loadedItem.StartPap, id: Id + "StartCopy" );
            }

            ImGui.Text( "End Pap Path: " );
            ImGui.SameLine();
            DisplayPath( loadedItem.EndPap );
            if( !string.IsNullOrEmpty( loadedItem.EndPap ) ) {
                if( ImGui.Button( "SELECT" + Id + "End" ) ) {
                    Dialog.Invoke( new SelectResult( SelectResultType.GameAction, "[ACTION] " + loadedItem.Name + " Start", loadedItem.EndPap ) );
                }
                ImGui.SameLine();
                Copy( loadedItem.EndPap, id: Id + "EndCopy" );
            }

            ImGui.Text( "Hit Pap Path: " );
            ImGui.SameLine();
            DisplayPath( loadedItem.HitPap );
            if( !string.IsNullOrEmpty( loadedItem.HitPap ) ) {
                if( ImGui.Button( "SELECT" + Id + "Hit" ) ) {
                    Dialog.Invoke( new SelectResult( SelectResultType.GameAction, "[ACTION] " + loadedItem.Name + " Start", loadedItem.HitPap ) );
                }
                ImGui.SameLine();
                Copy( loadedItem.HitPap, id: Id + "HitCopy" );
            }
        }

        protected override string UniqueRowTitle( XivPap item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}
