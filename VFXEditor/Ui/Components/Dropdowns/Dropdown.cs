using Dalamud.Interface;
using ImGuiNET;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Components {
    public abstract class Dropdown<T> where T : class {
        protected readonly string Id;

        protected T Selected = null;
        protected readonly bool AllowNew;
        protected readonly bool AllowDelete;
        protected readonly List<T> Items;

        public Dropdown( string id, List<T> items, bool allowNew, bool allowDelete ) {
            Id = id;
            Items = items;
            AllowNew = allowNew;
            AllowDelete = allowDelete;
        }

        protected abstract void DrawSelected();

        protected abstract string GetText( T item, int idx );

        protected abstract void OnNew();

        protected abstract void OnDelete( T item );

        public void ClearSelected() { Selected = null; }

        protected virtual void DrawSelectItem( T item, int idx ) {
            var isColored = DoColor( item, out var col );

            using var color = ImRaii.PushColor( ImGuiCol.Text, col, isColored );
            using var _ = ImRaii.PushId( idx );

            if( ImGui.Selectable( GetText( item, idx ), item == Selected ) ) Selected = item;
        }

        protected virtual bool DoColor( T item, out Vector4 color ) {
            color = new( 1 );
            return false;
        }

        public void Draw() {
            using var _ = ImRaii.PushId( Id );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            if( Selected != null && !Items.Contains( Selected ) ) Selected = null;

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 2, 4 ) ) ) {
                var leftRightSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.ChevronLeft ) - 5;
                var inputSize = UiUtils.GetOffsetInputSize( leftRightSize * 2 );

                DrawLeftRight();

                ImGui.SameLine();
                ImGui.SetNextItemWidth( inputSize );
                DrawCombo();
            }

            if( AllowNew ) DrawNew();
            if( AllowDelete ) DrawDelete();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            using var __ = ImRaii.PushId( Items.IndexOf( Selected ) );

            if( Selected != null ) DrawSelected();
            else ImGui.Text( "Select an item..." );
        }

        private void DrawLeftRight() {
            using var font = ImRaii.PushFont( UiBuilder.IconFont );
            var index = Selected == null ? -1 : Items.IndexOf( Selected );

            if( UiUtils.DisabledTransparentButton( FontAwesomeIcon.ChevronLeft.ToIconString(), new Vector4( 1 ), Selected != null && index > 0 ) ) {
                Selected = Items[index - 1];
            }

            ImGui.SameLine();
            if( UiUtils.DisabledTransparentButton( FontAwesomeIcon.ChevronRight.ToIconString(), new Vector4( 1 ), Selected != null && index < ( Items.Count - 1 ) ) ) {
                Selected = Items[index + 1];
            }
        }

        private void DrawCombo() {
            Vector4 col = new( 1 );
            var isColored = Selected != null && DoColor( Selected, out col );
            using var color = ImRaii.PushColor( ImGuiCol.Text, col, isColored );
            using var combo = ImRaii.Combo( "##Combo", Selected == null ? "[NONE]" : GetText( Selected, Items.IndexOf( Selected ) ) );
            if( !combo ) return;
            if( isColored ) color.Pop();

            for( var idx = 0; idx < Items.Count; idx++ ) DrawSelectItem( Items[idx], idx );
        }

        private void DrawNew() {
            using var font = ImRaii.PushFont( UiBuilder.IconFont );
            ImGui.SameLine();
            if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) OnNew();
        }

        private void DrawDelete() {
            if( Selected != null ) {
                using var font = ImRaii.PushFont( UiBuilder.IconFont );
                ImGui.SameLine();
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 4 );
                if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) && Items.Contains( Selected ) ) {
                    OnDelete( Selected );
                    Selected = null;
                }
            }
        }
    }
}
