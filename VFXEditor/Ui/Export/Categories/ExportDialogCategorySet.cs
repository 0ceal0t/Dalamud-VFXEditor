using System.Collections.Generic;
using System.Linq;
using VfxEditor.FileManager.Interfaces;

namespace VfxEditor.Ui.Export.Categories {
    public class ExportDialogCategorySet {
        public readonly List<ExportDialogCategory> Categories = new();

        public ExportDialogCategorySet() {
            Categories.AddRange( Plugin.Managers.Where( x => x != null ).Select( x => new ExportDialogCategory( x ) ) );
        }

        public void Draw() => Categories.ForEach( x => x.Draw() );

        public void Reset() => Categories.ForEach( x => x.Reset() );

        public void RemoveDocument( IFileDocument document ) => Categories.ForEach( x => x.RemoveDocument( document ) );

        public Dictionary<string, string> Export( string modFolder, string groupOption ) {
            var ret = new Dictionary<string, string>();
            foreach( var category in Categories ) {
                foreach( var item in category.GetItemsToExport() ) item.PenumbraExport( modFolder, groupOption, ret );
            }
            return ret;
        }
    }
}