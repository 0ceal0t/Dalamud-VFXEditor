using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Select.Formats;
using VfxEditor.Utils;

namespace VfxEditor.Formats.ScdFormat {
    public class ScdManager : FileManager<ScdDocument, ScdFile, WorkspaceMetaBasic> {
        public ScdManager( ScdManagerGroup group ) : base( group ) {
            SourceSelect = new ScdSelectDialog( "Scd Select [LOADED]", this, true );
            ReplaceSelect = new ScdSelectDialog( "Scd Select [REPLACED]", this, false );
        }

        protected override ScdDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override ScdDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => new( this, NewWriteLocation, localPath, data );
    }
}
