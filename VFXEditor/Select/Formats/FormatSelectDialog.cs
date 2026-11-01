using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Select.Base;

namespace VfxEditor.Select.Formats {
    public class FormatSelectDialog : SelectDialog {
        public FormatSelectDialog( string id, string extension, FileManagerBase manager, bool isSourceDialog, Func<SelectDialog, IEnumerable<SelectTab>> tabs ) :
            base( id, extension, manager, isSourceDialog ) {
            GameTabs.AddRange( tabs( this ) );
        }
    }
}