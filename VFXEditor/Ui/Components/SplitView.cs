using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Ui.Components {
    public abstract class SplitView<T> where T : class {
        protected readonly string Id;

        protected bool AllowNew;
        protected bool DrawOnce = false;

        protected readonly List<T> Items;
        protected T Selected = null;

        public SplitView( string id, List<T> items, bool allowNew ) {
            Id = id;
            Items = items;
            AllowNew = allowNew;
        }

        protected virtual void DrawControls() { }

        protected virtual void DrawLeftColumn() {
            if( Selected != null && !Items.Contains( Selected ) ) Selected = null;
            for( var idx = 0; idx < Items.Count; idx++ ) {
                if( DrawLeftItem( Items[idx], idx ) ) break; // break if list modified
            }
        }

        protected virtual void DrawRightColumn() {
            if( Selected == null ) ImGui.TextDisabled( "Select an item..." );
            else DrawSelected();
        }

        protected abstract bool DrawLeftItem( T item, int idx );

        protected abstract void DrawSelected();

        public void Draw() {
            using var _ = ImRaii.PushId( Id );
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0, 4 ) ) ) {
                ImGui.Columns( 2, "Columns", true );
            }

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 4, 4 ) ) ) {
                if( AllowNew ) DrawControls();
            }

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0, 4 ) ) )
            using( var left = ImRaii.Child( "Left" ) ) {
                style.Pop();
                DrawLeftColumn();
            }

            if( !DrawOnce ) {
                ImGui.SetColumnWidth( 0, 200 );
                DrawOnce = true;
            }
            ImGui.NextColumn();

            using( var right = ImRaii.Child( "Right" ) ) {
                DrawRightColumn();
            }

            ImGui.Columns( 1 );
        }
    }
}
