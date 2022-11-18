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

        public abstract List<T> GetItems();

        protected abstract string GetName( T item, int idx );

        protected abstract void OnNew();

        protected abstract void OnDelete( T item );

        protected void DrawDropdown( string id, bool separatorBefore = true ) {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            if( separatorBefore) {
                ImGui.Separator();
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            }

            var items = GetItems();
            if( Selected != null && !items.Contains( Selected ) ) Selected = null;

            if( ImGui.BeginCombo( $"{id}-Selected", Selected == null ? "[NONE]" : GetName( Selected, items.IndexOf( Selected ) ) ) ) {
                for( var idx = 0; idx < items.Count; idx++ ) {
                    var item = items[idx];
                    if( ImGui.Selectable( $"{GetName( item, idx )}{id}{idx}", item == Selected ) ) Selected = item;
                }
                ImGui.EndCombo();
            }

            if( AllowNew ) {
                ImGui.PushFont( UiBuilder.IconFont );
                ImGui.SameLine();
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Plus}{id}" ) ) OnNew();

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

        public void ClearSelected() { Selected = null; }
    }
}
