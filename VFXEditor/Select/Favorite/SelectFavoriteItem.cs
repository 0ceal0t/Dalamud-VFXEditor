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
    }
}