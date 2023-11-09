using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.FileBrowser.Filter {
    public class FileBrowserFilter {
        public string Filter;
        public HashSet<string> CollectionFilters;

        public bool Empty() => string.IsNullOrEmpty( Filter ) && ( ( CollectionFilters == null ) || ( CollectionFilters.Count == 0 ) );

        public bool Matches( string filter ) => ( Filter == filter ) || ( CollectionFilters != null && CollectionFilters.Contains( filter ) );

        public bool HasExtension( IEnumerable<string> extensions ) => extensions.Any( HasExtension );

        public bool HasExtension( string extension ) =>
             Filter != null && Filter.ToLower().Equals( $".{extension.ToLower()}" ) ||
            (
                CollectionFilters != null &&
                CollectionFilters.Any( x => x.ToLower().Equals( $".{extension.ToLower()}" ) )
            );
    }
}
