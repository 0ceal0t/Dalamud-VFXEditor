using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Components {
    public abstract class Dropdown<T> where T : class {
        protected T Selected = null;
        protected readonly bool AllowNew;
        protected readonly List<T> Items;

        public Dropdown( List<T> items, bool allowNew ) {
            Items = items;
            AllowNew = allowNew;
        }

        protected abstract string GetText( T item, int idx );

        protected abstract void OnNew();

        protected abstract void OnDelete( T item );

        public void ClearSelected() { Selected = null; }

        public virtual void Draw( string id ) {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            if( Selected != null && !Items.Contains( Selected ) ) Selected = null;

            if( ImGui.BeginCombo( $"{id}-Selected", Selected == null ? "[NONE]" : GetText( Selected, Items.IndexOf( Selected ) ) ) ) {
                for( var idx = 0; idx < Items.Count; idx++ ) {
                    var item = Items[idx];
                    if( ImGui.Selectable( $"{GetText( item, idx )}{id}{idx}", item == Selected ) ) Selected = item;
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
    }
}
