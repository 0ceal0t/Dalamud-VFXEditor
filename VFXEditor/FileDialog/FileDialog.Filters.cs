using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FDialog {
    public partial class FileDialog {
        private struct FilterStruct {
            public string Filter;
            public HashSet<string> CollectionFilters;

            public void Clear() {
                Filter = "";
                CollectionFilters.Clear();
            }

            public bool Empty() {
                return string.IsNullOrEmpty(Filter) && ((CollectionFilters == null) ? true : (CollectionFilters.Count == 0));
            }

            public bool FilterExists(string filter) {
                return ( Filter == filter ) || (CollectionFilters != null && CollectionFilters.Contains( filter ));
            }
        }

        private List<FilterStruct> Filters = new();
        private FilterStruct SelectedFilter;

        public static Regex FilterRegex = new Regex( @"[^,{}]+(\{([^{}]*?)\})?", RegexOptions.Compiled );

        private void ParseFilters(string filters) {
            // ".*,.cpp,.h,.hpp"
            // "Source files{.cpp,.h,.hpp},Image files{.png,.gif,.jpg,.jpeg},.md"

            Filters.Clear();
            if( filters.Length == 0 ) return;

            var currentFilterFound = false;
            MatchCollection matches = FilterRegex.Matches( filters );
            foreach( Match m in matches ) {
                var match = m.Value;
                FilterStruct filter = new();

                if(match.Contains("{")) {
                    var exts = m.Groups[2].Value;
                    filter = new FilterStruct
                    {
                        Filter = match.Split( '{' )[0],
                        CollectionFilters = new HashSet<string>( exts.Split( ',' ) )
                    };
                }
                else {
                    filter = new FilterStruct
                    {
                        Filter = match,
                        CollectionFilters = new()
                    };
                }
                Filters.Add( filter );

                if(!currentFilterFound && filter.Filter == SelectedFilter.Filter) {
                    currentFilterFound = true;
                    SelectedFilter = filter;
                }
            }

            if(!currentFilterFound && !(Filters.Count == 0)) {
                SelectedFilter = Filters[0];
            }
        }

        private void SetSelectedFilterWithExt(string ext) {
            if( Filters.Count == 0 ) return;
            if( string.IsNullOrEmpty( ext ) ) return;

            foreach(var filter in Filters) {
                if(filter.FilterExists(ext)) {
                    SelectedFilter = filter;
                }
            }

            if( SelectedFilter.Empty()) {
                SelectedFilter = Filters[0];
            }
        }

        private void ApplyFilteringOnFileList() {
            lock( FilesLock ) {
                FilteredFiles.Clear();

                foreach( var file in Files ) {
                    var show = true;
                    if( !string.IsNullOrEmpty( SearchTag ) && !file.FileName_Optimized.Contains( SearchTag ) && !file.FileName.Contains( SearchTag ) ) {
                        show = false;
                    }

                    if( IsDirectoryMode() && file.Type != FileStructType.Directory ) {
                        show = false;
                    }

                    if( show ) {
                        FilteredFiles.Add( file );
                    }
                }
            }
        }
    }
}
