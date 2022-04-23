using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using Dalamud.Logging;
using ImGuiNET;
using VFXSelect.Select.Rows;

namespace VFXSelect.PAP {
    public class PAPActionSelect : PAPSelectTab<XivActionPap, XivActionPapSelected> {
        private ImGuiScene.TextureWrap Icon;

        public PAPActionSelect( string parentId, string tabId, PAPSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.ActionPap, dialog ) {
        }

        protected override bool CheckMatch( XivActionPap item, string searchInput ) {
            return Matches( item.Name, searchInput );
        }

        protected override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        protected override void DrawSelected( XivActionPapSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.ActionPap.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( Icon != null && Icon.ImGuiHandle != IntPtr.Zero ) {
                ImGui.Image( Icon.ImGuiHandle, new Vector2( Icon.Width, Icon.Height ) );
            }

            DrawDict( loadedItem.StartAnimations, "Start", loadedItem.ActionPap.Name );

            DrawDict( loadedItem.EndAnimations, "End", loadedItem.ActionPap.Name );

            DrawDict( loadedItem.HitAnimations, "Hit", loadedItem.ActionPap.Name );
        }

        protected override string UniqueRowTitle( XivActionPap item ) {
            return item.Name + "##" + item.RowId;
        }

        private void DrawDict(Dictionary<string, string> items, string label, string name) {
            foreach( var item in items ) {
                var skeleton = item.Key;
                var path = item.Value;
                ImGui.Text( $"{label} ({skeleton}): " );
                ImGui.SameLine();
                if( path.Contains( "action.pap" ) ) {
                    DisplayPathWarning( path, "Be careful about modifying this file, as it contains dozens of animations for every job" );
                }
                else {
                    DisplayPath( path );
                }
                if( !string.IsNullOrEmpty( path ) ) {
                    if( ImGui.Button( $"SELECT{Id}-{label}-{skeleton}" ) ) {
                        Dialog.Invoke( new SelectResult( SelectResultType.GameAction, $"[ACTION] {name} {label} ({skeleton})", path ) );
                    }
                    ImGui.SameLine();
                    Copy( path, id: $"{Id}-{label}-Copy-{skeleton}" );
                }
            }
        }
    }
}
