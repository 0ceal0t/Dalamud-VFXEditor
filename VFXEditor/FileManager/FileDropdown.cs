using Dalamud.Interface;
using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract class FileDropdown<T> where T : class {
        protected T Selected = null;

        private readonly bool AllowNew;

        public FileDropdown( bool allowNew ) {
            AllowNew = allowNew;
        }

        protected abstract List<T> GetOptions();

        protected abstract string GetName( T item, int idx );

        protected abstract void OnNew();

        protected abstract void OnDelete( T item );

        protected void DrawDropdown( string id, bool separatorBefore = true ) {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            if( separatorBefore) {
                ImGui.Separator();
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            }

            var options = GetOptions();
            if( ImGui.BeginCombo( $"{id}-Selected", Selected == null ? "[NONE]" : GetName( Selected, options.IndexOf( Selected ) ) ) ) {
                for( var i = 0; i < options.Count; i++ ) {
                    var option = options[i];
                    if( ImGui.Selectable( $"{GetName( option, i )}{id}{i}", option == Selected ) ) {
                        Selected = option;
                    }
                }
                ImGui.EndCombo();
            }

            if( AllowNew ) {
                ImGui.PushFont( UiBuilder.IconFont );
                ImGui.SameLine();
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Plus}{id}" ) ) {
                    OnNew();
                }
                if( Selected != null ) {
                    ImGui.SameLine();
                    ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 20 );
                    if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}{id}" ) ) {
                        OnDelete( Selected );
                        Selected = null;
                    }
                }
                ImGui.PopFont();
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
        }
    }
}
