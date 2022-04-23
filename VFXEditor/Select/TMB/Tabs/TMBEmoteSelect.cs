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
    public class TMBEmoteSelect : TMBSelectTab<XivEmoteTmb, XivEmoteTmb> {
        private ImGuiScene.TextureWrap Icon;

        public TMBEmoteSelect( string parentId, string tabId, TMBSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.EmoteTmb, dialog ) {
        }

        protected override bool CheckMatch( XivEmoteTmb item, string searchInput ) {
            return Matches( item.Name, searchInput );
        }

        protected override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        protected override void DrawSelected( XivEmoteTmb loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( Icon != null && Icon.ImGuiHandle != IntPtr.Zero ) {
                ImGui.Image( Icon.ImGuiHandle, new Vector2( Icon.Width, Icon.Height ) );
            }

            for( var i = 0; i < loadedItem.TmbFiles.Count; i++) {
                var path = loadedItem.TmbFiles[i];

                ImGui.Text( $"Tmb Path #{i}: " );
                ImGui.SameLine();
                DisplayPath( path );
                if( !string.IsNullOrEmpty( path ) ) {
                    if( ImGui.Button( $"SELECT{Id}-{i}" ) ) {
                        Dialog.Invoke( new SelectResult( SelectResultType.GameAction, $"[EMOTE] {loadedItem.Name} #{i}", path ) );
                    }
                    ImGui.SameLine();
                    Copy( path, id: $"{Id}-Copy-{i}" );
                }
            }
        }

        protected override string UniqueRowTitle( XivEmoteTmb item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}
