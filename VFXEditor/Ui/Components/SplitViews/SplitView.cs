using ImGuiNET;
using OtterGui.Raii;
using System.Numerics;

namespace VfxEditor.Ui.Components.SplitViews {
    public abstract class SplitView<T> where T : class {
        protected readonly string Id;
        protected T Selected = null;

        protected bool DrawOnce = false;
        protected int InitialWidth = 200;

        public SplitView( string id ) {
            Id = id;
        }

        protected abstract void DrawLeftColumn();

        protected abstract void DrawRightColumn();

        protected abstract void DrawPreLeft();

        public virtual void Draw() {
            using var _ = ImRaii.PushId( Id );

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0, 4 ) ) ) {
                ImGui.Columns( 2, "Columns", true );
                DrawPreLeft();

                using var left = ImRaii.Child( "Left" );
                style.Pop();

                DrawLeftColumn();
            }

            if( !DrawOnce ) {
                ImGui.SetColumnWidth( 0, InitialWidth );
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
