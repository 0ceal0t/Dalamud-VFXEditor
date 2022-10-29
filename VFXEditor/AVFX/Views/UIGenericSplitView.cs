using ImGuiNET;

namespace VfxEditor.AVFX.VFX {
    public abstract class UIGenericSplitView : IUIBase {
        public bool AllowNew;
        public bool AllowDelete;
        private bool DrawOnce = false;

        public UIGenericSplitView( bool allowNew, bool allowDelete ) {
            AllowNew = allowNew;
            AllowDelete = allowDelete;
        }

        public abstract void DrawLeftCol( string parentId );
        public abstract void DrawRightCol( string parentId );
        public abstract void DrawControls( string parentId );

        public void DrawInline( string parentId = "" ) {
            ImGui.Columns( 2, parentId + "/Cols", true );
            DrawControls( parentId );

            ImGui.BeginChild( parentId + "/Tree" );
            DrawLeftCol( parentId );
            ImGui.EndChild();

            if( !DrawOnce ) {
                ImGui.SetColumnWidth( 0, 200 );
                DrawOnce = true;
            }
            ImGui.NextColumn();

            ImGui.BeginChild( parentId + "/Split" );
            DrawRightCol( parentId );
            ImGui.EndChild();

            ImGui.Columns( 1 );
        }
    }
}
