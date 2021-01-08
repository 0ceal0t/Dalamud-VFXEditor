using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dalamud.Interface;
using Dalamud.Plugin;
using ImGuiNET;

namespace VFXEditor.UI {
    public class UISavedDialog {
        public Plugin _plugin;
        public string id;
        public bool Load;

        public Action<SavedItem> OnSelect;
        public bool Visible = false;
        public string SearchInput;
        public string NameInput;
        public SavedItemType Type;

        public bool ValidSelect = false;
        public SavedItem Selected;

        public UISavedDialog(Plugin plugin, string _id, bool load = false ) {
            _plugin = plugin;
            id = _id;
            Load = load;
        }

        public void Show(SavedItemType type) {
            Type = type;
            Visible = true;
            OnSelect = null;
            ValidSelect = false;
        }

        public bool DrawOnce = false;
        public void Draw() {
            if( !Visible )
                return;
            if( !DrawOnce ) {
                ImGui.SetNextWindowSize( new Vector2( 800, 500 ) );
                DrawOnce = true;
            }
            // =====================
            var ret = ImGui.Begin( "Saved Items##" + id, ref Visible );
            if( !ret )
                return;

            ImGui.InputText( "Search##" + id, ref SearchInput, 255 );
            float footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();
            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), false );

            int idx = 0;
            foreach(var item in _plugin.Configuration.SavedItems.Values ) {
                if(!VFXSelectDialog.Matches(SearchInput, item.Name) || item.Type != Type ) { idx++; continue; }
                if( ImGui.Selectable(item.Name + "##" + id + "/" + idx, Selected.Path == item.Path )) {
                    if(Selected.Path != item.Path ) {
                        Selected = item;
                        ValidSelect = true;
                    }
                }
                idx++;
            }

            ImGui.EndChild();
            ImGui.Separator();
            if( !Load ) {
                ImGui.InputText( "Name##" + id, ref NameInput, 255 );
                ImGui.SameLine();
            }
            if( ValidSelect ) {
                if( ImGui.Button( "Select##" + id ) ) {
                    OnSelect?.Invoke( Selected );
                }
            }
            ImGui.End();
        }
    }
}
