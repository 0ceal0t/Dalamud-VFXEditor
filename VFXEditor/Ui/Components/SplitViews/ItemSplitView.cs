using ImGuiNET;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;

namespace VfxEditor.Ui.Components.SplitViews {
    public abstract class ItemSplitView<T> : SplitView<T> where T : class {
        protected readonly List<T> Items;
        protected bool ShowControls;

        public ItemSplitView( string id, List<T> items, bool showControls ) : base( id ) {
            Items = items;
            ShowControls = showControls;
        }

        protected virtual void DrawControls() { }

        protected override void DrawLeftColumn() {
            if( Selected != null && !Items.Contains( Selected ) ) Selected = null;
            for( var idx = 0; idx < Items.Count; idx++ ) {
                if( DrawLeftItem( Items[idx], idx ) ) break; // break if list modified
            }
        }

        protected override void DrawRightColumn() {
            if( Selected == null ) ImGui.TextDisabled( "Select an item..." );
            else DrawSelected();
        }

        protected abstract bool DrawLeftItem( T item, int idx );
        protected abstract void DrawSelected();

        protected override void DrawPreLeft() {
            if( !ShowControls ) return;
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
            DrawControls();
        }
    }
}
