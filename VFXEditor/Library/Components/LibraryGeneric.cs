using Dalamud.Bindings.ImGui;

namespace VfxEditor.Library.Components {
    public abstract class LibraryGeneric {
        public LibraryFolder Parent;
        public string Name;
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
    }
}