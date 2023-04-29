using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
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

        protected virtual void DrawSelectItem( T item, string id, int idx ) {
            var isColored = DoColor( item, out var color );
            if( isColored ) ImGui.PushStyleColor( ImGuiCol.Text, color );

            if( ImGui.Selectable( $"{GetText( item, idx )}{id}{idx}", item == Selected ) ) Selected = item;

            if( isColored ) ImGui.PopStyleColor( 1 ); // Uncolor
        }

        protected virtual bool DoColor( T item, out Vector4 color ) {
            color = new( 1 );
            return false;
        }

        public virtual void Draw( string id ) {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            if( Selected != null && !Items.Contains( Selected ) ) Selected = null;

            ImGui.PushStyleVar( ImGuiStyleVar.ItemSpacing, new Vector2( 2, 4 ) );

            var leftRightSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.ChevronLeft ) - 5;
            var inputSize = UiUtils.GetOffsetInputSize( leftRightSize * 2 );

            ImGui.PushFont( UiBuilder.IconFont );

            var index = Selected == null ? -1 : Items.IndexOf( Selected );
            if( UiUtils.DisabledTransparentButton( $"{( char )FontAwesomeIcon.ChevronLeft}{id}-Left", new Vector4( 1 ), Selected != null && index > 0 ) ) {
                Selected = Items[index - 1];
            }
            ImGui.SameLine();
            if( UiUtils.DisabledTransparentButton( $"{( char )FontAwesomeIcon.ChevronRight}{id}-Right", new Vector4( 1 ), Selected != null && index < ( Items.Count - 1 ) ) ) {
                Selected = Items[index + 1];
            }
            ImGui.PopFont();

            ImGui.SameLine();
            ImGui.SetNextItemWidth( inputSize );

            Vector4 color = new( 1 );
            var isColored = Selected != null && DoColor( Selected, out color );
            if( isColored ) ImGui.PushStyleColor( ImGuiCol.Text, color );
            if( ImGui.BeginCombo( $"{id}-Selected", Selected == null ? "[NONE]" : GetText( Selected, Items.IndexOf( Selected ) ) ) ) {
                if( isColored ) ImGui.PopStyleColor( 1 ); // Uncolor
                for( var idx = 0; idx < Items.Count; idx++ ) DrawSelectItem( Items[idx], id, idx );
                ImGui.EndCombo();
            }
            else if( isColored ) ImGui.PopStyleColor( 1 ); // Uncolor

            ImGui.PopStyleVar( 1 );

            if( AllowNew ) {
                ImGui.PushFont( UiBuilder.IconFont );
                ImGui.SameLine();
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Plus}{id}" ) ) OnNew();

                if( Selected != null ) {
                    ImGui.SameLine();
                    ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 4 );
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
