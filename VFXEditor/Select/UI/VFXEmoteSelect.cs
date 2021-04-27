using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Plugin;
using ImGuiNET;
using VFXSelect.Data.Rows;

namespace VFXSelect.UI {
    public class VFXEmoteSelect : VFXSelectTab<XivEmote, XivEmoteSelected> {
        public VFXEmoteSelect( string parentId, string tabId, SheetManager sheet, VFXSelectDialog dialog ) : 
            base( parentId, tabId, sheet._Emotes, sheet._pi, dialog ) {
        }

        public override bool CheckMatch( XivEmote item, string searchInput ) {
            return VFXSelectDialog.Matches( item.Name, searchInput );
        }

        ImGuiScene.TextureWrap Icon;
        public override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        public override void DrawSelected( XivEmoteSelected loadedItem ) {
            if(loadedItem == null) { return; }
            ImGui.Text( loadedItem.Emote.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( Icon != null ) {
                ImGui.Image( Icon.ImGuiHandle, new Vector2( Icon.Width, Icon.Height ) );
            }

            int vfxIdx = 0;
            foreach( var _vfx in loadedItem.VfxPaths ) {
                ImGui.Text( "VFX #" + vfxIdx + ": " );
                ImGui.SameLine();
                _dialog.DisplayPath( _vfx );
                if( ImGui.Button( "SELECT" + Id + vfxIdx ) ) {
                    _dialog.Invoke( new VFXSelectResult( VFXSelectType.GameEmote, "[EMOTE] " + loadedItem.Emote.Name + " #" + vfxIdx, _vfx ) );
                }
                ImGui.SameLine();
                _dialog.Copy( _vfx, id: Id + "Copy" + vfxIdx );
                vfxIdx++;
            }
        }

        public override string UniqueRowTitle( XivEmote item ) {
            return item.Name + Id + item.RowId;
        }
    }
}