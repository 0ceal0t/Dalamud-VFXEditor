using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace VfxEditor.FileBrowser.Filter {
    public partial class FileBrowserFilters {
        private FileBrowserFilter Selected;
        private readonly FileBrowserDialog Dialog;
        public readonly List<FileBrowserFilter> Filters = [];

        [GeneratedRegex( "[^,{}]+(\\{([^{}]*?)\\})?", RegexOptions.Compiled )]
        private static partial Regex FilterRegexPattern();
        private static readonly Regex FilterRegex = FilterRegexPattern();

        public FileBrowserFilters( FileBrowserDialog dialog, string filters ) {
            Dialog = dialog;

            // ".*,.cpp,.h,.hpp"
            // "Source files{.cpp,.h,.hpp},Image files{.png,.gif,.jpg,.jpeg},.md"

            if( string.IsNullOrEmpty( filters ) ) return;

            var matches = FilterRegex.Matches( filters );
            foreach( var m in matches.Cast<Match>() ) {
                var match = m.Value;

                if( match.Contains( '{' ) ) {
                    Filters.Add( new FileBrowserFilter {
                        Filter = match.Split( '{' )[0],
                        CollectionFilters = new HashSet<string>( m.Groups[2].Value.Split( ',' ) )
                    } );
                }
                else {
                    Filters.Add( new FileBrowserFilter {
                        Filter = match,
                        CollectionFilters = []
                    } );
                }
            }

            if( Filters.Count > 0 ) Selected = Filters[0];
        }

        public void Draw() {
            if( Filters.Count == 0 ) return;

            ImGui.SameLine();
            ImGui.SetNextItemWidth( 200 );

            var text = Selected == null ? "[NONE]" : Selected.Text;

            using var combo = ImRaii.Combo( "##Filters", text, ImGuiComboFlags.None );
            if( !combo ) return;

            foreach( var (filter, idx) in Filters.WithIndex() ) {
                using var _ = ImRaii.PushId( idx );
                if( ImGui.Selectable( filter.Text, filter == Selected ) ) {
                    Selected = filter;
                    Dialog.UpdateFiles();
                }
            }
        }

        public bool FilterOut( string extension ) {
            if( string.IsNullOrEmpty( extension ) ) return false;
            if( Selected == null ) return false;
            if( Filters.Count > 0 && !Selected.Empty() && !Selected.Matches( extension.ToLower() ) && Selected.Filter != ".*" ) return true;
            return false;
        }

        public void SetSelectedFilter( string extension ) {
            if( Filters.Count == 0 ) return;
            if( string.IsNullOrEmpty( extension ) ) return;

            foreach( var filter in Filters ) {
                if( filter.Matches( extension ) ) Selected = filter;
            }

            CheckFilters();
        }

        public void CheckFilters() {
            if( ( Selected == null || Selected.Empty() ) && Filters.Count > 0 ) Selected = Filters[0];
        }

        public string ApplySelectedFilterExtension( string result ) {
            if( Selected == null ) return result;

            // a collection like {.cpp, .h}, so can't decide on an extension
            if( Selected.CollectionFilters != null && Selected.CollectionFilters.Count > 0 ) return result;

            // a single one, like .cpp
            if( !Selected.Filter.Contains( '*' ) && result != Selected.Filter ) {
                var lastPoint = result.LastIndexOf( '.' );
                if( lastPoint != -1 ) result = result[..lastPoint];
                result += Selected.Filter;
            }
            return result;
        }
    }
}
