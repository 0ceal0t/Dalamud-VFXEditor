using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Components.SplitViews {
    public abstract class ItemSplitView<T> : SplitView<T> where T : class {
        public ItemSplitView( string id, List<T> items ) : base( id, items ) { }

        protected virtual void DrawControls() { }

        protected override void DrawLeftColumn() {
            if( Selected != null && !Items.Contains( Selected ) ) Selected = null;
            for( var idx = 0; idx < Items.Count; idx++ ) {
                if( DrawLeftItem( Items[idx], idx ) ) break; // break if list modified
            }
        }

        protected abstract bool DrawLeftItem( T item, int idx );

        protected override void DrawRightColumn() {
            if( Selected == null ) ImGui.TextDisabled( "Select an item..." );
            else DrawSelected();
        }

        protected abstract void DrawSelected();

        protected override void DrawPreLeft() {
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
            DrawControls();
        }

        // ===========

        protected void DrawNewDeleteControls( Action onNew, Action<T> onDelete ) {
            using var font = ImRaii.PushFont( UiBuilder.IconFont );

            if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) onNew();

            using var disabled = ImRaii.Disabled( Selected == null );

            ImGui.SameLine();
            if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) {
                onDelete( Selected );
                Selected = null;
            }
        }
    }
}
