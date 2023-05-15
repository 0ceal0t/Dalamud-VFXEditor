using ImGuiNET;

namespace VfxEditor.NodeLibrary {
    public abstract class AvfxNodeLibraryGeneric {
        public AvfxNodeLibraryFolder Parent;

        public AvfxNodeLibraryGeneric( AvfxNodeLibraryFolder parent ) {
            Parent = parent;
        }

        public abstract bool Draw( AvfxNodeLibrary library, string searchInput );

        public abstract bool Matches( string input );

        public abstract void Cleanup();

        public abstract AvfxNodeLibraryProps ToProps();

        public abstract bool Contains( AvfxNodeLibraryGeneric item );

        protected void DragDrop( AvfxNodeLibrary library, string text, ref bool listModified ) {
            if( ImGui.BeginDragDropSource( ImGuiDragDropFlags.None ) ) {
                library.StartDragging( this );
                ImGui.Text( text );
                ImGui.EndDragDropSource();
            }

            if( ImGui.BeginDragDropTarget() ) {
                if( library.StopDragging( this ) ) listModified = true;
                ImGui.EndDragDropTarget();
            }
        }
    }
}
