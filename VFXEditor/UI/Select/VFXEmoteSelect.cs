using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Plugin;
using ImGuiNET;

namespace VFXEditor.UI {
    public class VFXEmoteSelect : VFXSelectTab<XivEmote, XivEmoteSelected> {
        public VFXEmoteSelect( string parentId, string tabId, List<XivEmote> data, Plugin plugin, VFXSelectDialog dialog ) : base( parentId, tabId, data, plugin, dialog ) {
        }

        public override bool CheckMatch( XivEmote item, string searchInput ) {
            return VFXSelectDialog.Matches( item.Name, searchInput );
        }

        public override void DrawSelected( XivEmoteSelected loadedItem ) {
            if(loadedItem == null) { return; }
            ImGui.Text( loadedItem.Emote.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

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

        public override void Load() {
            _plugin.Manager.LoadEmote();
        }

        public override bool ReadyCheck() {
            return _plugin.Manager.EmoteLoaded;
        }

        public override bool SelectItem( XivEmote item, out XivEmoteSelected loadedItem ) {
            return _plugin.Manager.SelectEmote( item, out loadedItem );
        }

        public override string UniqueRowTitle( XivEmote item ) {
            return item.Name + Id + item.RowId;
        }
    }
}