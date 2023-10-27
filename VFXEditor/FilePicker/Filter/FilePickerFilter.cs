using System.Collections.Generic;

namespace VfxEditor.FilePicker.Filter {
    public class FilePickerFilter {
        public string Filter;
        public HashSet<string> CollectionFilters;

        public bool Empty() => string.IsNullOrEmpty( Filter ) && ( ( CollectionFilters == null ) || ( CollectionFilters.Count == 0 ) );

        public bool Matches( string filter ) => ( Filter == filter ) || ( CollectionFilters != null && CollectionFilters.Contains( filter ) );
    }
}
