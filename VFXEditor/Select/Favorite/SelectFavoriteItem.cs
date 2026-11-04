using System;
using System.Collections.Generic;
using VfxEditor.Utils;

namespace VfxEditor.Select.Favorite {
    [Serializable]
    public class SelectFavoriteItem {
        public string Id = UiUtils.RandomString( 10 );
        public string DragDropName => IsFolder ? FolderName : ( Result?.DisplayString ?? string.Empty );

        public SelectResult Result;

        public bool IsFolder = false;
        public string FolderName = "New Folder";
        public List<SelectFavoriteItem> Items = [];

        public bool Matches( string searchInput ) {
            if( string.IsNullOrEmpty( searchInput ) ) return true;
            if( IsFolder ) return true;
            if( Result.Path.Contains( searchInput, StringComparison.CurrentCultureIgnoreCase ) ) return true;
            if( ( string.IsNullOrEmpty( Result.Name ) ? Result.DisplayString : Result.Name ).Contains( searchInput, StringComparison.CurrentCultureIgnoreCase ) ) return true;
            return false;
        }
    }
}