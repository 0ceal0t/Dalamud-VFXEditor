using System.Collections.Generic;
using System.Linq;
using VfxEditor.FileManager.Interfaces;

namespace VfxEditor.Ui.Export.Categories {
    public class ExportDialogCategorySet {
        public readonly List<ExportDialogCategory> Categories = [];

        public ExportDialogCategorySet() {
            Categories.AddRange( Plugin.Groups.Where( x => x != null ).Select( x => new ExportDialogCategory( x ) ) );
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

        public List<object> ExportManipulations() {
            var ret = new List<object>();
            foreach( var category in Categories ) {
                foreach( var item in category.GetItemsToExport() ) item.PenumbraExportManipulations( ret );
            }
            return ret;
        }

        public Dictionary<string, string> WorkspaceExport() {
            var ret = new Dictionary<string, string>();
            foreach( var category in Categories ) category.WorkspaceExport( ret );
            return ret;
        }

        public void WorkspaceImport( Dictionary<string, string> files, Dictionary<IFileManagerGroup, int> offsets ) {
            foreach( var category in Categories ) category.WorkspaceImport( files, offsets );
        }
    }
}