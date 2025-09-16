using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Ui.Components.Base;

namespace VfxEditor.Ui.Components.SplitViews {
    public abstract class SplitView<T> : SelectView<T> where T : class {
        protected bool DrawOnce = false;
        protected int InitialWidth = 200;

        public SplitView( string id, List<T> items ) : base( id, items ) { }

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
