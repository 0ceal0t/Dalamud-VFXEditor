using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System.Numerics;

namespace VfxEditor.Select.Base {
    public abstract class SelectTabBase {
        protected readonly SelectDialog Dialog;
        protected readonly string Name;

        protected Vector2 DefaultWindowPadding = new();
        protected string SearchInput = "";

        public SelectTabBase( SelectDialog dialog, string name ) {
            Dialog = dialog;
            Name = name;
        }

        protected virtual void DrawControls() { }

        protected abstract int NumColumns();

        protected abstract void DrawTable();

        public void Draw() {
            using var _ = ImRaii.PushId( Name );

            using var tabItem = ImRaii.TabItem( Name );
            if( !tabItem ) return;

            DefaultWindowPadding = ImGui.GetStyle().WindowPadding;

            ImGui.InputTextWithHint( "##Search", "Search", ref SearchInput, 255 );

            DrawControls();

            using var style = ImRaii.PushStyle( ImGuiStyleVar.CellPadding, new Vector2( 4, 3 ) );
            using var padding = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) );
            using var child = ImRaii.Child( "Child", new Vector2( -1, -1 ), true );
            using var table = ImRaii.Table( "Table", NumColumns(), ImGuiTableFlags.RowBg | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.NoSavedSettings );
            if( !table ) return;
            padding.Dispose();

            DrawTable();
        }
    }
}