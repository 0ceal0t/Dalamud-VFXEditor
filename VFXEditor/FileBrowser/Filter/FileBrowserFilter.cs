using System;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.FileBrowser.Filter {
    public class FileBrowserFilter {
        public string Filter;
        public HashSet<string> CollectionFilters;

        public string Text {
            get {
                if( Filter == ".*" ) return "All Files";
                if( CollectionFilters?.Count > 0 ) return Filter + " (" + string.Join( ", ", CollectionFilters ) + ")";
                return Filter;
            }
        }

        public bool Empty() => string.IsNullOrEmpty( Filter ) && ( ( CollectionFilters == null ) || ( CollectionFilters.Count == 0 ) );

        public bool Matches( string filter ) => ( Filter == filter ) || ( CollectionFilters != null && CollectionFilters.Contains( filter ) );

        public bool HasExtension( IEnumerable<string> extensions ) => extensions.Any( HasExtension );

        public bool HasExtension( string extension ) =>
             Filter != null && string.Equals( Filter, $".{extension}", StringComparison.OrdinalIgnoreCase ) ||
            (
                CollectionFilters != null &&
                CollectionFilters.Any( x => string.Equals( x, $".{extension}", StringComparison.OrdinalIgnoreCase ) )
            );
    }
}