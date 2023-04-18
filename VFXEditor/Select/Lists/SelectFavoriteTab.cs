using System;
using System.Collections.Generic;

namespace VfxEditor.Select.Lists {
    [Serializable]
    public class SelectFavorite {
        // Same fields as SelectResult so it gets serialized properly
        public SelectResultType Type;
        public string DisplayString;
        public string Path;

        [NonSerialized]
        public bool Editing = false;

        public SelectResult Result => new() {
            Type = Type,
            DisplayString = DisplayString,
            Path = Path
        };

        public bool ResultEquals( SelectResult other ) => other == Result;
    }

    public class SelectFavoriteTab : SelectListTab<SelectResult> {
        public SelectFavoriteTab( SelectDialog dialog, string name, List<SelectResult> items ) : base( dialog, name, items ) { }

        protected override SelectResult ItemToSelectResult( SelectResult item ) => item;
    }
}
