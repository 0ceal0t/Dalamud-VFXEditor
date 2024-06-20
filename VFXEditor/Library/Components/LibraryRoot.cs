using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;

namespace VfxEditor.Library.Components {
    public class LibraryRoot : LibraryFolder {
        public readonly string RootName;
        public readonly List<LibraryProps> Props;

        public LibraryRoot( string rootName, List<LibraryProps> props ) : base( null, "", "", props ) {
            RootName = rootName;
            Props = props;
        }

        public void Save() {
            Props.Clear();
            Props.AddRange( ChildrenToProps() );
        }

        public void Draw( LibraryManager manager, string searchInput, ref LibraryRoot lastDrawnRoot ) {
            using var tab = ImRaii.TabItem( RootName );
            if( !tab ) return;

            lastDrawnRoot = this;
            Draw( manager, searchInput );
        }
    }
}
