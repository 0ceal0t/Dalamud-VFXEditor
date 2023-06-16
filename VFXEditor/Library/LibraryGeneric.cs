using ImGuiNET;

namespace VfxEditor.Library {
    public abstract class LibraryGeneric {
        public LibraryFolder Parent;
        protected string Name;
        protected readonly string Id;

        public LibraryGeneric( LibraryFolder parent, string name, string id ) {
            Parent = parent;
            Name = name;
            Id = id;
        }

        public abstract bool Draw( LibraryManager library, string searchInput );

        public abstract bool Matches( string input );

        public abstract void Cleanup();

        public abstract LibraryProps ToProps();

        public abstract bool Contains( LibraryGeneric item );

        protected void DragDrop( LibraryManager library, string text, ref bool listModified ) {
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
