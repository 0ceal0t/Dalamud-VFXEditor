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
    public class PapActionSelect : PapSelectTab<XivPap, XivPapSelected> {
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

        protected override void DrawSelected( XivPapSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Pap.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( Icon != null && Icon.ImGuiHandle != IntPtr.Zero ) {
                ImGui.Image( Icon.ImGuiHandle, new Vector2( Icon.Width, Icon.Height ) );
            }

            DrawDict( loadedItem.StartAnimations, "Start", loadedItem.Pap.Name );

            DrawDict( loadedItem.EndAnimations, "End", loadedItem.Pap.Name );

            DrawDict( loadedItem.HitAnimations, "Hit", loadedItem.Pap.Name );
        }

        protected override string UniqueRowTitle( XivPap item ) {
            return item.Name + "##" + item.RowId;
        }

        private void DrawDict(Dictionary<string, string> items, string label, string name) {
            foreach( var item in items ) {
                var skeleton = item.Key;
                var path = item.Value;
                ImGui.Text( $"{label} ({skeleton}): " );
                ImGui.SameLine();
                DisplayPath( path );
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
